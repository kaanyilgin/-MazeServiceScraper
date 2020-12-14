﻿// <auto-generated />
using System;
using MazeServiceScraper.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MazeServiceScraper.Infrastructure.Migrations
{
    [DbContext(typeof(MazeDbContext))]
    [Migration("20201214022353_CreatedDateColumnRemoved")]
    partial class CreatedDateColumnRemoved
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MazeServiceScraper.Infrastructure.Database.Cast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Birthday");

                    b.Property<int>("CastId");

                    b.Property<string>("Name");

                    b.Property<int?>("ShowId");

                    b.HasKey("Id");

                    b.HasIndex("ShowId");

                    b.ToTable("Casts");
                });

            modelBuilder.Entity("MazeServiceScraper.Infrastructure.Database.Show", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("ShowId");

                    b.HasKey("Id");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("MazeServiceScraper.Infrastructure.Database.Cast", b =>
                {
                    b.HasOne("MazeServiceScraper.Infrastructure.Database.Show")
                        .WithMany("Casts")
                        .HasForeignKey("ShowId");
                });
#pragma warning restore 612, 618
        }
    }
}
