﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ShopMaster.Areas.BackEnd.Models;

public partial class ShopmasterdbContext : DbContext
{
    public ShopmasterdbContext()
    {
    }

    public ShopmasterdbContext(DbContextOptions<ShopmasterdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminGroup> AdminGroups { get; set; }

    public virtual DbSet<MenuGroup> MenuGroups { get; set; }

    public virtual DbSet<MenuSub> MenuSubs { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Rule> Rules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=192.168.0.82;database=shopmasterdb;user id=ShopMasterDB;password=Alex0310", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.GroupId, "GroupId");

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
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

            entity.ToTable("AdminGroup");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<MenuGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("MenuGroup");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.SortOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
        });

        modelBuilder.Entity<MenuSub>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("MenuSub");

            entity.HasIndex(e => e.GroupId, "GroupId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.GroupId).HasColumnType("int(11)");
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Route).HasMaxLength(255);
            entity.Property(e => e.SortOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.Group).WithMany(p => p.MenuSubs)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("MenuSub_ibfk_1");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Product");

            entity.HasIndex(e => e.TypeId, "TypeId");

            entity.Property(e => e.Id).HasColumnType("bigint(20)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Publish).HasDefaultValueSql("'1'");
            entity.Property(e => e.Stock).HasColumnType("int(11)");
            entity.Property(e => e.TypeId).HasColumnType("int(11)");

            entity.HasOne(d => d.Type).WithMany(p => p.Products)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Product_ibfk_1");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ProductType");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Rule>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.MenuId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("Rule");

            entity.HasIndex(e => e.MenuId, "MenuId");

            entity.Property(e => e.GroupId).HasColumnType("int(11)");
            entity.Property(e => e.MenuId).HasColumnType("int(11)");
            entity.Property(e => e.CanCreate).HasDefaultValueSql("'0'");
            entity.Property(e => e.CanDelete).HasDefaultValueSql("'0'");
            entity.Property(e => e.CanRead).HasDefaultValueSql("'1'");
            entity.Property(e => e.CanUpdate).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Group).WithMany(p => p.Rules)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("Rule_ibfk_1");

            entity.HasOne(d => d.Menu).WithMany(p => p.Rules)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("Rule_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
