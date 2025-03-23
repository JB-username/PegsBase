﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PegsBase.Data;

#nullable disable

namespace PegsBase.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250321125706_AddPegRegisterTable")]
    partial class AddPegRegisterTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PegsBase.Models.PegRegister", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("GradeElevation")
                        .HasColumnType("numeric");

                    b.Property<string>("Locality")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("SurveyDate")
                        .HasColumnType("date");

                    b.Property<string>("Surveyor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("XCoord")
                        .HasColumnType("numeric");

                    b.Property<decimal>("YCoord")
                        .HasColumnType("numeric");

                    b.Property<decimal>("ZCoord")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("PegRegister");
                });
#pragma warning restore 612, 618
        }
    }
}
