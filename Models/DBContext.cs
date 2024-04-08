using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Capstone.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
        }

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Ordini> Ordini { get; set; }
        public virtual DbSet<Vini> Vini { get; set; }
        public virtual DbSet<OrdVini> OrdVini { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ordini>()
                .Property(e => e.Totale)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ordini>()
                .Property(e => e.UserId)
                .IsFixedLength();

            modelBuilder.Entity<Vini>()
                .Property(e => e.Prezzo)
                .HasPrecision(10, 2);
        }
    }
}
