using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Project.Areas.Identity.Data
{
    public partial class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Bill> Bills { get; set; } = null!;
        public virtual DbSet<BillItem> BillItems { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<CartDetail> CartDetails { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Provider> Providers { get; set; } = null!;
        public virtual DbSet<Receive> Receives { get; set; } = null!;
        public virtual DbSet<RecevieDetail> RecevieDetails { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategorys { get; set; } = null!;
        public virtual DbSet<ApplicationUser> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ECommerce;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("BILL");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.BuyingDate)
                    .HasColumnType("datetime")
                    .HasColumnName("BUYING_DATE");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("USERID")
                    .IsRequired(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("NAME");

                entity.Property(e => e.Note).HasColumnName("NOTE");

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .HasColumnName("PHONE");

                entity.Property(e => e.Status).HasColumnName("STATUS");

                entity.Property(e => e.Total)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("TOTAL");

                entity.HasOne(d => d.User)
                   .WithMany()
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_BILL_APPLICATION_USER");
            });

            modelBuilder.Entity<BillItem>(entity =>
            {
                entity.ToTable("BILL_ITEM");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.IdBill)
                    .HasMaxLength(10)
                    .HasColumnName("ID_BILL");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("NAME");

                entity.Property(e => e.Number).HasColumnName("NUMBER");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("PRICE");

                entity.Property(e => e.UrlImage).HasColumnName("URL_IMAGE");

                entity.HasOne(d => d.IdBillNavigation)
                    .WithMany(p => p.BillItems)
                    .HasForeignKey(d => d.IdBill)
                    .HasConstraintName("FK_BILL_ITEM_BILL");
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.ToTable("BLOG");

                entity.Property(e => e.Conten).HasColumnName("CONTEN");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("DATE");

                entity.Property(e => e.Title)
                    .HasMaxLength(200)
                    .HasColumnName("TITLE");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("NAMEPRODUCT");

                entity.Property(e => e.Status).HasColumnName("STATUS");

                entity.HasMany(d => d.Products)
                    .WithOne(p => p.ProductCategory)
                    .HasForeignKey(d => d.IdCategory);
            });
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.IdCategory)
                    .HasMaxLength(10)
                    .HasColumnName("IdCategory");

                entity.Property(e => e.Category)
                    .HasMaxLength(50)
                    .HasColumnName("CATEGORY");

                entity.Property(e => e.NameProduct)
                    .HasMaxLength(50)
                    .HasColumnName("NAMEPRODUCT");

                entity.Property(e => e.Number).HasColumnName("NUMBER");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("PRICE");

                entity.Property(e => e.Review).HasColumnName("REVIEW");

                entity.Property(e => e.Sales).HasColumnName("SALES");

                entity.Property(e => e.Status).HasColumnName("STATUS");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("CART");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("USERID")
                    .IsRequired(false);

                entity.Property(e => e.Status).HasColumnName("STATUS");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CART_APPLICATION_USER");
            });

            modelBuilder.Entity<CartDetail>(entity =>
            {
                entity.ToTable("CART_DETAIL");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.IdProduct)
                    .HasMaxLength(10)
                    .HasColumnName("IDProduct");

                entity.Property(e => e.Idcart)
                    .HasMaxLength(10)
                    .HasColumnName("IDCART");

                entity.Property(e => e.Number).HasColumnName("NUMBER");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("PRICE");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.CartDetails)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CART_DETAIL_Product");

                entity.HasOne(d => d.IdcartNavigation)
                    .WithMany(p => p.CartDetails)
                    .HasForeignKey(d => d.Idcart)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CART_DETAIL_CART");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("IMAGE");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.IdProduct)
                    .HasMaxLength(10)
                    .HasColumnName("IDProduct");

                entity.Property(e => e.IsMain).HasColumnName("IS_MAIN");

                entity.Property(e => e.UrlImage).HasColumnName("URL_IMAGE");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.IdProduct)
                    .HasConstraintName("FK_IMAGE_Product");
            });

            modelBuilder.Entity<Provider>(entity =>
            {
                entity.ToTable("PROVIDER");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<Receive>(entity =>
            {
                entity.ToTable("RECEIVE");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("DATE");

                entity.Property(e => e.Provider)
                    .HasMaxLength(10)
                    .HasColumnName("PROVIDER");

                entity.HasOne(d => d.ProviderNavigation)
                    .WithMany(p => p.Receives)
                    .HasForeignKey(d => d.Provider)
                    .HasConstraintName("FK_RECEIVE_PROVIDER");
            });

            modelBuilder.Entity<RecevieDetail>(entity =>
            {
                entity.ToTable("RECEVIE_DETAIL");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID");

                entity.Property(e => e.IdProduct)
                    .HasMaxLength(10)
                    .HasColumnName("ID_PRODUCT");

                entity.Property(e => e.IdReceive)
                    .HasMaxLength(10)
                    .HasColumnName("ID_RECEIVE");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("PRICE");

                entity.Property(e => e.Number).HasColumnName("NUMBER");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.RecevieDetails)
                    .HasForeignKey(d => d.IdProduct)
                    .HasConstraintName("FK_RECEVIE_DETAIL_Product");

                entity.HasOne(d => d.IdReceiveNavigation)
                    .WithMany(p => p.RecevieDetails)
                    .HasForeignKey(d => d.IdReceive)
                    .HasConstraintName("FK_RECEVIE_DETAIL_RECEIVE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
