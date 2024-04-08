﻿using MotoRental.Core.Entity.SchemaCore;
using MotoRental.Infrastructure.Repository.Interface;
using MotoRental.Infrastructure.Repository.Settings;
using MotoRental.Infrastructure.Repository.Settings.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Infrastructure.Repository
{
    public class CNHTypeRepository : DomainRepository<CNHType>, ICNHTypeRepository
    {
        public CNHTypeRepository(IDbConnection databaseConnection) : base(databaseConnection)
        {
        }

        public CNHTypeRepository(IDatabaseFactory databaseOptions) : base(databaseOptions)
        {
        }
    }
}