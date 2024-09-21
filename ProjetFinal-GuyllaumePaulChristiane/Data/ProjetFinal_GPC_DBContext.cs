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
    public class ProjetFinal_GPC_DBContext : IdentityDbContext<User>
    {
        public ProjetFinal_GPC_DBContext (DbContextOptions<ProjetFinal_GPC_DBContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);//apporter une modification spécifique
            modelBuilder.Entity<DVD>()
                .HasIndex(dvd => dvd.TitreFrancais).IsUnique();
            //Default values for user preferences
            modelBuilder.Entity<User>()
                .Property(u => u.nbDVDParPage)
                .HasDefaultValue(12);
            modelBuilder.Entity<User>()
                .Property(u => u.courrielOnAppropriation)
                .HasDefaultValue(true);
            modelBuilder.Entity<User>()
                .Property(u => u.courrielOnDVDCreate)
                .HasDefaultValue(true);
            modelBuilder.Entity<User>()
                .Property(u => u.courrielOnDVDDelete)
                .HasDefaultValue(true);

            modelBuilder.Entity<User>()
                .ToTable(u => u.HasCheckConstraint("CK_DVD_per_pages", "nbDVDParPage >= 6 AND nbDVDParPage <= 99"));
        }
        public DbSet<DVD> DVDs { get; set; }

    }
}
