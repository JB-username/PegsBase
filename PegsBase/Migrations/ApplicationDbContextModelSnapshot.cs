﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PegsBase.Data;

#nullable disable

namespace PegsBase.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("PegsBase.Models.Identity.Invite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AssignedRole")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("PegsBase.Models.Identity.WhitelistedEmails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WhitelistedEmails");
                });

            modelBuilder.Entity("PegsBase.Models.PegRegister", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FromPeg")
                        .HasColumnType("text");

                    b.Property<decimal?>("GradeElevation")
                        .HasColumnType("numeric");

                    b.Property<bool>("HasPegCalc")
                        .HasColumnType("boolean");

                    b.Property<int>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Locality")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<bool>("PegFailed")
                        .HasColumnType("boolean");

                    b.Property<string>("PegName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PointType")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("SurveyDate")
                        .HasColumnType("date");

                    b.Property<string>("Surveyor")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<decimal>("XCoord")
                        .HasColumnType("numeric");

                    b.Property<decimal>("YCoord")
                        .HasColumnType("numeric");

                    b.Property<decimal>("ZCoord")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("PegRegister");
                });

            modelBuilder.Entity("PegsBase.Models.RawSurveyData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("BackBearingReturn")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BackCheckHorizontalDifference")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BackCheckHorizontalDistance")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BackCheckPegElevations")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BackCheckVerticalError")
                        .HasColumnType("numeric");

                    b.Property<string>("BackSightPeg")
                        .HasColumnType("text");

                    b.Property<decimal>("BacksightPegX")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BacksightPegY")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BacksightPegZ")
                        .HasColumnType("numeric");

                    b.Property<decimal>("DeltaX")
                        .HasColumnType("numeric");

                    b.Property<decimal>("DeltaY")
                        .HasColumnType("numeric");

                    b.Property<decimal>("DeltaZ")
                        .HasColumnType("numeric");

                    b.Property<string>("ForeSightPeg")
                        .HasColumnType("text");

                    b.Property<decimal>("ForwardBearing")
                        .HasColumnType("numeric");

                    b.Property<decimal>("ForwardBearingReturn")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleDirectArc1Backsight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleDirectArc1Foresight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleDirectArc2Backsight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleDirectArc2Foresight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleDirectReducedArc1")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleDirectReducedArc2")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleMeanArc1")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleMeanArc2")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleMeanFinal")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleMeanFinalReturn")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleTransitArc1Backsight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleTransitArc1Foresight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleTransitArc2Backsight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleTransitArc2Foresight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleTransitReducedArc1")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HAngleTransitReducedArc2")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HorizontalDistanceBacksight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HorizontalDistanceForesight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("InstrumentHeight")
                        .HasColumnType("numeric");

                    b.Property<string>("Locality")
                        .HasColumnType("text");

                    b.Property<decimal>("NewPegX")
                        .HasColumnType("numeric");

                    b.Property<decimal>("NewPegY")
                        .HasColumnType("numeric");

                    b.Property<decimal>("NewPegZ")
                        .HasColumnType("numeric");

                    b.Property<bool>("PegFailed")
                        .HasColumnType("boolean");

                    b.Property<decimal>("SlopeDistanceBacksight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("SlopeDistanceForesight")
                        .HasColumnType("numeric");

                    b.Property<string>("StationPeg")
                        .HasColumnType("text");

                    b.Property<decimal>("StationPegX")
                        .HasColumnType("numeric");

                    b.Property<decimal>("StationPegY")
                        .HasColumnType("numeric");

                    b.Property<decimal>("StationPegZ")
                        .HasColumnType("numeric");

                    b.Property<DateOnly>("SurveyDate")
                        .HasColumnType("date");

                    b.Property<string>("Surveyor")
                        .HasColumnType("text");

                    b.Property<decimal>("TargetHeightBacksight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TargetHeightForesight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleBacksightMeanArc1")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleBacksightMeanArc2")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleBacksightMeanFinal")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleDirectArc1Backsight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleDirectArc1Foresight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleDirectArc2Backsight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleDirectArc2Foresight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleForesightMeanArc1")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleForesightMeanArc2")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleForesightMeanFinal")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleTransitArc1Backsight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleTransitArc1Foresight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleTransitArc2Backsight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VAngleTransitArc2Foresight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VerticalDifferenceBacksight")
                        .HasColumnType("numeric");

                    b.Property<decimal>("VerticalDifferenceForesight")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("RawSurveyData");
                });

            modelBuilder.Entity("PegsBase.Models.SurveyNote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AbandonmentReason")
                        .HasColumnType("boolean");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsAbandoned")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSigned")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Locality")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NoteType")
                        .HasColumnType("integer");

                    b.Property<string>("ThumbnailPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UploadedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UploadedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SurveyNotes");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
