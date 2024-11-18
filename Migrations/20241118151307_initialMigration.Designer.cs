﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using passapi.data;

#nullable disable

namespace passapi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241118151307_initialMigration")]
    partial class initialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("passapi.models.TestResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<int>("FifthGoalRank")
                        .HasColumnType("integer");

                    b.Property<int>("FirstGoalRank")
                        .HasColumnType("integer");

                    b.Property<int>("FourthGoalRank")
                        .HasColumnType("integer");

                    b.Property<string>("Held")
                        .HasColumnType("text");

                    b.Property<int>("HewittPercentile")
                        .HasColumnType("integer");

                    b.Property<int>("NationalPercentile")
                        .HasColumnType("integer");

                    b.Property<int>("OverallRank")
                        .HasColumnType("integer");

                    b.Property<int>("PercentCorrect")
                        .HasColumnType("integer");

                    b.Property<int>("RawScore")
                        .HasColumnType("integer");

                    b.Property<string>("Response")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RitScore")
                        .HasColumnType("integer");

                    b.Property<int>("SecondGoalRank")
                        .HasColumnType("integer");

                    b.Property<int>("SeventhGoalRank")
                        .HasColumnType("integer");

                    b.Property<int>("SixthGoalRank")
                        .HasColumnType("integer");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.Property<int>("Subject")
                        .HasColumnType("integer");

                    b.Property<Guid>("TestId")
                        .HasColumnType("uuid");

                    b.Property<int>("ThirdGoalRank")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("TestResults");
                });
#pragma warning restore 612, 618
        }
    }
}
