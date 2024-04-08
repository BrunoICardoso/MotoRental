using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Settings.Interface
{
    public interface IDataSettings
    {
        string ContextBase { get; set; }
    }
}
