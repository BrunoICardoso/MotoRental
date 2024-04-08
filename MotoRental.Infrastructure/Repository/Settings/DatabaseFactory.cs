using Microsoft.Extensions.Options;
using MotoRental.Core.Appsettings;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Settings
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private IOptions<DataSettings> _dataSettings;
        public IDbConnection GetDbConnection => new NpgsqlConnection(_dataSettings.Value.ContextBase);

        public DatabaseFactory(IOptions<DataSettings> dataSettings)
        {
            _dataSettings = dataSettings;
        }
    }
}
