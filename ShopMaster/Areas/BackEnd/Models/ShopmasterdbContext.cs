using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class shopmasterdbContext : DbContext
{
    public shopmasterdbContext()
    {
    }

    public shopmasterdbContext(DbContextOptions<shopmasterdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminGroup> AdminGroups { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Ecoupon> Ecoupons { get; set; }

    public virtual DbSet<EcouponEvent> EcouponEvents { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberType> MemberTypes { get; set; }

    public virtual DbSet<MenuGroup> MenuGroups { get; set; }

    public virtual DbSet<MenuSub> MenuSubs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PayInfo> PayInfos { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Rule> Rules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=219.68.168.127;database=shopmasterdb;user id=ShopMasterDB;password=Alex0310", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.10-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("Admin")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.GroupId, "GroupId");

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GroupId).HasColumnType("int(11)");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Group).WithMany(p => p.Admins)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("Admin_ibfk_1");
        });

        modelBuilder.Entity<AdminGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("AdminGroup")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Cart");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("bigint(20)");
            entity.Property(e => e.MemberId).HasColumnType("bigint(20)");
            entity.Property(e => e.ProductId).HasColumnType("bigint(20)");
        });

        modelBuilder.Entity<Ecoupon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("ECoupon")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Code, "Code").IsUnique();

            entity.HasIndex(e => e.EcouponEventId, "ECouponEventId");

            entity.HasIndex(e => e.MemberId, "MemberId");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.EcouponEventId)
                .HasColumnType("bigint(20)")
                .HasColumnName("ECouponEventId");
            entity.Property(e => e.MemberId).HasColumnType("bigint(20)");
            entity.Property(e => e.OrderId).HasColumnType("bigint(20)");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(1)");

            entity.HasOne(d => d.EcouponEvent).WithMany(p => p.Ecoupons)
                .HasForeignKey(d => d.EcouponEventId)
                .HasConstraintName("ECoupon_ibfk_1");

            entity.HasOne(d => d.Member).WithMany(p => p.Ecoupons)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("ECoupon_ibfk_2");

            entity.HasOne(d => d.Order).WithMany(p => p.Ecoupons)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("ECoupon_ibfk_3");
        });

        modelBuilder.Entity<EcouponEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("ECouponEvent")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.CodePrefix, "CodePrefix").IsUnique();

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CodePrefix).HasMaxLength(50);
            entity.Property(e => e.CouponType)
                .HasDefaultValueSql("'Discount'")
                .HasColumnType("enum('Discount','FreeShipping')");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Discount).HasPrecision(10, 2);
            entity.Property(e => e.ExpiryDays).HasColumnType("int(11)");
            entity.Property(e => e.MinOrderAmount).HasPrecision(10, 2);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("Member")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.MemberTypeId, "MemberTypeId");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(1)");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.MemberTypeId)
                .HasDefaultValueSql("'4'")
                .HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.MemberType).WithMany(p => p.Members)
                .HasForeignKey(d => d.MemberTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Member_ibfk_1");
        });

        modelBuilder.Entity<MemberType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("MemberType")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Discount)
                .HasPrecision(3, 2)
                .HasDefaultValueSql("'1.00'");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpgradeThreshold).HasPrecision(10, 2);
        });

        modelBuilder.Entity<MenuGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("MenuGroup")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.SortOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<MenuSub>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("MenuSub")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.GroupId, "GroupId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.GroupId).HasColumnType("int(11)");
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Route).HasMaxLength(255);
            entity.Property(e => e.SortOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("Order")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.PaymentType, "FK_Order_PaymentType");

            entity.HasIndex(e => e.MemberId, "MemberId");

            entity.HasIndex(e => e.MemberTypeId, "MemberTypeId");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.MemberId).HasColumnType("bigint(20)");
            entity.Property(e => e.MemberTypeId)
                .HasDefaultValueSql("'4'")
                .HasColumnType("int(11)");
            entity.Property(e => e.PaymentType).HasColumnType("int(11)");
            entity.Property(e => e.Status).HasColumnType("int(11)");
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);

            entity.HasOne(d => d.Member).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("Order_ibfk_1");

            entity.HasOne(d => d.MemberType).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MemberTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Order_ibfk_2");

            entity.HasOne(d => d.PaymentTypeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_PaymentType");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("OrderDetail")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.FinalPrice).HasPrecision(10, 2);
            entity.Property(e => e.OrderId).HasColumnType("bigint(20)");
            entity.Property(e => e.OriginalPrice).HasPrecision(10, 2);
            entity.Property(e => e.ProductId).HasColumnType("bigint(20)");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)");
            entity.Property(e => e.SubTotal).HasPrecision(10, 2);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("OrderDetail_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("OrderDetail_ibfk_2");
        });

        modelBuilder.Entity<PayInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("PayInfo")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("Product")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.TypeId, "TypeId");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.MainImage).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Publish)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(1)");
            entity.Property(e => e.Stock).HasColumnType("int(11)");
            entity.Property(e => e.TypeId).HasColumnType("int(11)");
            entity.Property(e => e.Views)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.Type).WithMany(p => p.Products)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Product_ibfk_1");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("ProductImage")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.ProductId).HasColumnType("bigint(20)");
            entity.Property(e => e.SortOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("ProductImage_ibfk_1");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("ProductType")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Rule>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.MenuId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("Rule")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.MenuId, "MenuId");

            entity.Property(e => e.GroupId).HasColumnType("int(11)");
            entity.Property(e => e.MenuId).HasColumnType("int(11)");
            entity.Property(e => e.CanCreate).HasDefaultValueSql("'0'");
            entity.Property(e => e.CanDelete).HasDefaultValueSql("'0'");
            entity.Property(e => e.CanRead).HasDefaultValueSql("'1'");
            entity.Property(e => e.CanUpdate).HasDefaultValueSql("'0'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
