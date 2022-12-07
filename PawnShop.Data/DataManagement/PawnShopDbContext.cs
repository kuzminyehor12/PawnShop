using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PawnShop.Data.Models
{
    public partial class PawnShopDbContext : DbContext
    {
        public PawnShopDbContext()
        {
        }

        public PawnShopDbContext(DbContextOptions<PawnShopDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<ClientData> ClientData { get; set; }
        public virtual DbSet<PassportData> PassportData { get; set; }
        public virtual DbSet<Pawnings> Pawnings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PawnShopDb;Trusted_Connection=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.HasIndex(e => e.Name)
                    .HasName("UK_Categories")
                    .IsUnique();

                entity.Property(e => e.CategoryId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Note).HasColumnType("text");
            });

            modelBuilder.Entity<ClientData>(entity =>
            {
                entity.HasKey(e => e.ClientId);

                entity.Property(e => e.ClientId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Patronymic).HasMaxLength(50);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.PassportData)
                    .WithMany(p => p.ClientData)
                    .HasForeignKey(d => d.PassportDataId)
                    .OnDelete(DeleteBehavior.ClientCascade)
                    .HasConstraintName("FK_ClientData_PassportData");
            });

            modelBuilder.Entity<PassportData>(entity =>
            {
                entity.HasKey(e => e.PassportIdataId);

                entity.HasIndex(e => e.Number)
                    .HasName("UK_PassportData_Number")
                    .IsUnique();

                entity.Property(e => e.PassportIdataId)
                    .HasColumnName("PassportIDataId")
                    .ValueGeneratedNever();

                entity.Property(e => e.DateOfIssue).HasColumnType("date");

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Series)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Pawnings>(entity =>
            {
                entity.HasKey(e => e.PawningId);

                entity.Property(e => e.PawningId).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.ReturnDate).HasColumnType("date");

                entity.Property(e => e.SubmissionDate).HasColumnType("date");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Pawnings)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Pawnings_Categories");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Pawnings)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Pawnings_ClientData");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
