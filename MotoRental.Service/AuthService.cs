using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using MotoRental.Core.Appsettings;
using MotoRental.Core.DTO.AuthService;
using MotoRental.Core.Entity.SchemaAuth;
using MotoRental.Core.Enum;
using MotoRental.Core.Exceptions;
using MotoRental.Core.ResponseDefault;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Service.Interface;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MotoRental.Service
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public AuthService(IOptions<JwtSettings> jwtSettings, IUserRepository userRepository, IUserRoleRepository userRoleRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<ReturnAPI<AuthenticationDTO>> AuthenticateAsync(LoginDTO login)
        {
            try
            {
                User user = null;

                var usersExist = await _userRepository.AnyAsync();

                if (!usersExist)
                {
                    var errors = ValidatePassword(login.Password);

                    if (errors.Any())
                    {
                        throw new BadRequestException($"Password did not pass criteria: {string.Join(',', errors)} ");
                    }

                    var adminUser = new User
                    {
                        Username = login.Username,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(login.Password),
                        Active = true
                    };

                    user = await _userRepository.AddGetEntityAsync(adminUser);
                    await _userRepository.SaveChangesAsync();

                    await _userRoleRepository.AddAsync(new UserRole { IdUser = user.IdUser, IdRole = (int)RoleEnum.Admin });
                    await _userRoleRepository.SaveChangesAsync();

                }
                else
                {
                    user = await _userRepository.GetByIdAsync(w => w.Username == login.Username);

                    if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                    {
                        throw new NotFoundException("User not found or invalid password");
                    }
                }

                var userRoles = await _userRoleRepository.GetUserRolesAsync(user.IdUser);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, login.Username),
                    new Claim($"{nameof(user.IdUser)}", $"{user.IdUser}", ClaimValueTypes.Integer)
                }.Concat(userRoles.Select(role => new Claim(ClaimTypes.Role, role))).ToArray()

                    ),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                string tokenString = tokenHandler.WriteToken(token);


                return new ReturnAPI<AuthenticationDTO>(HttpStatusCode.OK, new AuthenticationDTO
                {
                    Token = tokenString,
                    ExpiresAt = token.ValidTo,
                    Roles = userRoles,
                    UserId = user.IdUser
                });

            }
            catch (Exception)
            {
                _userRoleRepository.Rollback();
                _userRepository.Rollback();
                throw;
            }
        }


        public async Task<ReturnAPI> AddUserAsync(UserCreateDTO userCreate)
        {
            try
            {
                var errors = ValidatePassword(userCreate.Password);

                if (errors.Any())
                {
                    throw new BadRequestException($"Password did not pass criteria: {string.Join(',', errors)} ");
                }


                var existingUser = await _userRepository.GetByIdAsync(w => w.Username == userCreate.Username);
                if (existingUser != null)
                {
                    throw new BadRequestException("Username already in use.");
                }


                var user = await _userRepository.AddGetEntityAsync(new User
                {
                    Username = userCreate.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreate.Password),
                    Active = true
                });
                await _userRepository.SaveChangesAsync();

                await _userRoleRepository.AddAsync(new UserRole { IdUser = user.IdUser, IdRole = (int)userCreate.Role });
                await _userRoleRepository.SaveChangesAsync();

                return new ReturnAPI(HttpStatusCode.Created)
                {
                    Message = "User created successfully."
                };
            }
            catch (Exception)
            {
                _userRoleRepository.Rollback();
                throw;
            }

        }


        public async Task<ReturnAPI> UpdateUserAsync(int userId, UserUpdateDTO userUpdate)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(w => w.IdUser == userId);
                if (user == null)
                {
                    throw new NotFoundException("User not found.");
                }

                if (!string.IsNullOrWhiteSpace(userUpdate.Username) && userUpdate.Username != user.Username)
                {
                    var existingUser = await _userRepository.GetByIdAsync(w => w.Username == userUpdate.Username);
                    if (existingUser != null)
                    {
                        throw new BadRequestException("Username already in use.");
                    }

                    user.Username = userUpdate.Username;
                }

                if (!string.IsNullOrWhiteSpace(userUpdate.Password))
                {
                    var errors = ValidatePassword(userUpdate.Password);
                    if (errors.Any())
                    {
                        throw new BadRequestException($"Password did not pass criteria: {string.Join(", ", errors)}");
                    }

                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userUpdate.Password);
                }

                if (userUpdate.Active != null)
                {
                    user.Active = userUpdate.Active.Value;
                }

                if (userUpdate.Role != null)
                {
                    await _userRoleRepository.DeleteAsync(w => w.IdRole == user.IdUser);
                    await _userRoleRepository.SaveChangesAsync();

                    await _userRoleRepository.AddAsync(new UserRole { IdUser = user.IdUser, IdRole = (int)userUpdate.Role });
                    await _userRoleRepository.SaveChangesAsync();
                }

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return new ReturnAPI(HttpStatusCode.OK)
                {
                    Message = "User updated successfully."
                };
            }
            catch (Exception)
            {
                _userRepository.Rollback();
                _userRoleRepository.Rollback();
                throw;
            }
        }

        public async Task<ReturnAPI<IEnumerable<UserDTO>>> GetAllUsersAsync(int? userId = null, string username = null)
        {
            var users = await _userRepository.GetUsersRoleAsync(userId, username);

            return new ReturnAPI<IEnumerable<UserDTO>>(HttpStatusCode.OK, users);
        }


        public async Task<ReturnAPI> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(w=> w.IdUser == userId);
                
                if (user == null)
                {
                    throw new NotFoundException("User not found.");
                }

                user.Active = false;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return new ReturnAPI(HttpStatusCode.OK)
                {
                    Message = "User disable successfully."
                };
            }
            catch (Exception)
            {
                _userRepository.Rollback();
                throw;
            }
        }


        #region Private

        private List<string> ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                errors.Add("Password must contain at least one uppercase letter.");
            }
            if (!Regex.IsMatch(password, "[!@#$%^&*(),.?\":{}|<>]"))
            {
                errors.Add("Password must contain at least one special character.");
            }
            if (password.Length < 6)
            {
                errors.Add("Password must be at least 6 characters long.");
            }

            return errors;
        }

        #endregion
    }
}

