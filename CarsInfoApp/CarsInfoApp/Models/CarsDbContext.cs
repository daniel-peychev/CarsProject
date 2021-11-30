using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CarsInfoApp.Models
{
    // DbContext provides a connection with the database via EF, using DbSets
    public partial class CarsDbContext : DbContext
    {
        public CarsDbContext()
        {
        }

        public CarsDbContext(DbContextOptions<CarsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Engine> Engines { get; set; }
        public virtual DbSet<Make> Makes { get; set; }
        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<User> Users { get; set; }

        //The custom class CarsDbContext overrides OnConfiguring method which is comming from the DbContext.
        //Here we can configure our coonection string
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=CarsDb;Trusted_Connection=True;");
            }
        }

        // This is the most powerful method of configuration and allows configuration to be specified without modifying your entity classes.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Engine>(entity =>
            {
                entity.ToTable("Engine");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EngineCode)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Fuel).HasMaxLength(64);

                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Engines)
                    .HasForeignKey(d => d.ModelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EngineModel");
            });

            modelBuilder.Entity<Make>(entity =>
            {
                entity.ToTable("Make");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EstablishmentDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.TotalCarsMade).HasDefaultValueSql("((0))");

                entity.Property(e => e.TotalIncome).HasColumnType("decimal(19, 4)");
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.ToTable("Model");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EndYear).HasColumnType("date");

                entity.Property(e => e.ModelName)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Price).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.StartYear).HasColumnType("date");

                entity.Property(e => e.TotalModelsMade).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Make)
                    .WithMany(p => p.Models)
                    .HasForeignKey(d => d.MakeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MakeModel");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
