using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository.Settings
{
    public abstract class DomainRepository<TEntity> : Repository<TEntity>,
                                                      IDomainRepository<TEntity> where TEntity : class
    {
        protected DomainRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        protected DomainRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }
    }
}
