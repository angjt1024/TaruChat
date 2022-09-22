﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaruChat.Data;

namespace TaruChat.Migrations
{
    [DbContext(typeof(ChatContext))]
    [Migration("20211125180636_MessageChat")]
    partial class MessageChat
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TaruChat.Models.Assign", b =>
                {
                    b.Property<string>("ClassID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SubjectID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ClassID", "SubjectID");

                    b.HasIndex("SubjectID");

                    b.ToTable("Assign");
                });

            modelBuilder.Entity("TaruChat.Models.Chat", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProfilePic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("Chat");
                });

            modelBuilder.Entity("TaruChat.Models.Class", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Class");
                });

            modelBuilder.Entity("TaruChat.Models.Enrollment", b =>
                {
                    b.Property<string>("ChatID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ChatID", "UserID");

                    b.HasIndex("UserID");

                    b.ToTable("Enrollment");
                });

            modelBuilder.Entity("TaruChat.Models.Message", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttachmentURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChatID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("MessageType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Word")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ChatID");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("TaruChat.Models.Subject", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Subject");
                });

            modelBuilder.Entity("TaruChat.Models.User", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ClassID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ClassID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("TaruChat.Models.Assign", b =>
                {
                    b.HasOne("TaruChat.Models.Class", "Class")
                        .WithMany("Assigns")
                        .HasForeignKey("ClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaruChat.Models.Subject", "Subject")
                        .WithMany("Assigns")
                        .HasForeignKey("SubjectID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Class");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("TaruChat.Models.Enrollment", b =>
                {
                    b.HasOne("TaruChat.Models.Chat", "Chat")
                        .WithMany("Enrollments")
                        .HasForeignKey("ChatID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TaruChat.Models.User", "User")
                        .WithMany("Enrollments")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TaruChat.Models.Message", b =>
                {
                    b.HasOne("TaruChat.Models.Chat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatID");

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("TaruChat.Models.User", b =>
                {
                    b.HasOne("TaruChat.Models.Class", "Class")
                        .WithMany()
                        .HasForeignKey("ClassID");

                    b.Navigation("Class");
                });

            modelBuilder.Entity("TaruChat.Models.Chat", b =>
                {
                    b.Navigation("Enrollments");
                });

            modelBuilder.Entity("TaruChat.Models.Class", b =>
                {
                    b.Navigation("Assigns");
                });

            modelBuilder.Entity("TaruChat.Models.Subject", b =>
                {
                    b.Navigation("Assigns");
                });

            modelBuilder.Entity("TaruChat.Models.User", b =>
                {
                    b.Navigation("Enrollments");
                });
#pragma warning restore 612, 618
        }
    }
}
