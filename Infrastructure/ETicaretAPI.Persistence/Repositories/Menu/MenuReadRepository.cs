﻿using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories
{
    public class MenuReadRepository :ReadRepository<Menu>, IMenuReadRepository
    {
        public MenuReadRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
   
}
