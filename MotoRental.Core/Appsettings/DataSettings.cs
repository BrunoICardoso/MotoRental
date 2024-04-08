using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Appsettings
{
    public class DataSettings : IDataSettings
    {
        public string ContextBase { get; set; }
    }
}
