﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ThePersonalBudgetApp.Context;

#nullable disable

namespace ThePersonalBudgetApp.Migrations
{
    [DbContext(typeof(BudgetDbContext))]
    [Migration("20241214162911_CreatedRelations2")]
    partial class CreatedRelations2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ThePersonalBudgetApp.DAL.Models.Budget", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("ThePersonalBudgetApp.DAL.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("BudgetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("BudgetId1")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExpenseBudgetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IncomeBudgetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("TotalAmount")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("BudgetId");

                    b.HasIndex("BudgetId1");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("ThePersonalBudgetApp.DAL.Models.Item", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("ThePersonalBudgetApp.DAL.Models.Category", b =>
                {
                    b.HasOne("ThePersonalBudgetApp.DAL.Models.Budget", null)
                        .WithMany("Expenses")
                        .HasForeignKey("BudgetId");

                    b.HasOne("ThePersonalBudgetApp.DAL.Models.Budget", null)
                        .WithMany("Incomes")
                        .HasForeignKey("BudgetId1");
                });

            modelBuilder.Entity("ThePersonalBudgetApp.DAL.Models.Item", b =>
                {
                    b.HasOne("ThePersonalBudgetApp.DAL.Models.Category", null)
                        .WithMany("Items")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ThePersonalBudgetApp.DAL.Models.Budget", b =>
                {
                    b.Navigation("Expenses");

                    b.Navigation("Incomes");
                });

            modelBuilder.Entity("ThePersonalBudgetApp.DAL.Models.Category", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
