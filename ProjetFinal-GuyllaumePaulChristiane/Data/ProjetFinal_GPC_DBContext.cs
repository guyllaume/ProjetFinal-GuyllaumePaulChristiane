using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetFinal_GuyllaumePaulChristiane.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ProjetFinal_GuyllaumePaulChristiane.Data
{
    public class ProjetFinal_GPC_DBContext : IdentityDbContext
    {
        public ProjetFinal_GPC_DBContext (DbContextOptions<ProjetFinal_GPC_DBContext> options)
            : base(options)
        {
        }

    }
}
