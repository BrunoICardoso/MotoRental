using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Settings.Interface
{
    public interface IDatabaseFactory
    {
        IDbConnection GetDbConnection { get; }
    }
}
