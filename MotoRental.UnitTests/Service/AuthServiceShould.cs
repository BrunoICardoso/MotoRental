using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using MotoRental.Core.Appsettings;
using MotoRental.Core.DTO.AuthService;
using MotoRental.Core.Entity.SchemaAuth;
using MotoRental.Core.Exceptions;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Service;
using MotoRental.Service.Interface;
using Xunit;


namespace MotoRental.UnitTests.Service
{
    public class AuthServiceShould
    {
        private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock;

        private readonly IAuthService _authService;

        public AuthServiceShould()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            _jwtSettingsMock.Setup(js => js.Value).Returns(new JwtSettings { SecretKey = "XGpLH1yanOi4w3MeodcweDMtKqgu0m7I", Issuer = "MotoRentalIssuer", Audience = "MotoRentalAudience" });

            _authService = new AuthService(_jwtSettingsMock.Object, _userRepositoryMock.Object, _userRoleRepositoryMock.Object);
        }


        [Fact]
        public async Task AuthenticateAsync_UserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.AnyAsync()).ReturnsAsync(true);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Expression<Func<User, bool>>>(), null, true)).ReturnsAsync((User)null);
            var loginDto = new LoginDTO { Username = "usuario", Password = "@Mo123456" };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _authService.AuthenticateAsync(loginDto));
        }
      

        [Fact]
        public async Task AuthenticateAsync_InvalidPasswordForFirstUser_ThrowsBadRequestException()
        {
            // Arrange
            var loginDto = new LoginDTO { Username = "newUser", Password = "weak" };
            _userRepositoryMock.Setup(x => x.AnyAsync()).ReturnsAsync(false); // Simula que não há usuários

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _authService.AuthenticateAsync(loginDto));
        }


    }
}
