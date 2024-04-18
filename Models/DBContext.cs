using Capstone.Controllers;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Capstone.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext2")
        {
        }
       
        public virtual DbSet<Ordini> Ordini { get; set; }
        public virtual DbSet<Pagamenti> Pagamenti { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Vini> Vini { get; set; }
        public virtual DbSet<OrdVini> OrdVini { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ordini>()
                .Property(e => e.Totale)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ordini>()
                .HasMany(e => e.OrdVini)
                .WithRequired(e => e.Ordini)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pagamenti>()
                .Property(e => e.TipoPagamento)
                .IsUnicode(false);

            modelBuilder.Entity<Pagamenti>()
                .HasMany(e => e.Ordini)
                .WithRequired(e => e.Pagamenti)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Ordini)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Vini>()
                .Property(e => e.Prezzo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Vini>()
                .HasMany(e => e.OrdVini)
                .WithRequired(e => e.Vini)
                .WillCascadeOnDelete(false);
        }
    }
}
