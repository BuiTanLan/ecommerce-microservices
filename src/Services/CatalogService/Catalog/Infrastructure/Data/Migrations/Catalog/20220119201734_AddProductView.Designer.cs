﻿// <auto-generated />
using System;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catalog.Infrastructure.Data.Migrations.Catalog
{
    [DbContext(typeof(CatalogDbContext))]
    [Migration("20220119201734_AddProductView")]
    partial class AddProductView
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Catalog.Categories.Category", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<long>("Version")
                        .HasColumnType("bigint")
                        .HasColumnName("version");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_categories_id");

                    b.ToTable("categories", "catalog");
                });

            modelBuilder.Entity("Catalog.Products.Product", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("AvailableStock")
                        .HasColumnType("integer")
                        .HasColumnName("available_stock");

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint")
                        .HasColumnName("category_id");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<int>("MaxStockThreshold")
                        .HasColumnType("integer")
                        .HasColumnName("max_stock_threshold");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(18,2)")
                        .HasColumnName("price");

                    b.Property<int>("RestockThreshold")
                        .HasColumnType("integer")
                        .HasColumnName("restock_threshold");

                    b.Property<long>("SupplierId")
                        .HasColumnType("bigint")
                        .HasColumnName("supplier_id");

                    b.Property<long>("Version")
                        .HasColumnType("bigint")
                        .HasColumnName("version");

                    b.HasKey("Id")
                        .HasName("pk_products");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_products_category_id");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_products_id");

                    b.HasIndex("SupplierId")
                        .HasDatabaseName("ix_products_supplier_id");

                    b.ToTable("products", "catalog");
                });

            modelBuilder.Entity("Catalog.Products.ProductView", b =>
                {
                    b.Property<long>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("product_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ProductId"));

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint")
                        .HasColumnName("category_id");

                    b.Property<string>("CategoryName")
                        .HasColumnType("text")
                        .HasColumnName("category_name");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("product_name");

                    b.Property<long?>("SupplierId")
                        .HasColumnType("bigint")
                        .HasColumnName("supplier_id");

                    b.Property<string>("SupplierName")
                        .HasColumnType("text")
                        .HasColumnName("supplier_name");

                    b.HasKey("ProductId")
                        .HasName("pk_product_views");

                    b.HasIndex("ProductId")
                        .IsUnique()
                        .HasDatabaseName("ix_product_views_product_id");

                    b.ToTable("product_views", "catalog");
                });

            modelBuilder.Entity("Catalog.Suppliers.Supplier", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("created_by");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_suppliers");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("ix_suppliers_id");

                    b.ToTable("suppliers", "catalog");
                });

            modelBuilder.Entity("Catalog.Products.Product", b =>
                {
                    b.HasOne("Catalog.Categories.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_products_categories_category_id");

                    b.HasOne("Catalog.Suppliers.Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_products_suppliers_supplier_id");

                    b.Navigation("Category");

                    b.Navigation("Supplier");
                });
#pragma warning restore 612, 618
        }
    }
}
