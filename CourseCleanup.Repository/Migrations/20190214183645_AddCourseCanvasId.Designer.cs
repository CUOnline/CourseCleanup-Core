﻿// <auto-generated />
using System;
using CourseCleanup.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CourseCleanup.Repository.Migrations
{
    [DbContext(typeof(CourseCleanupContext))]
    [Migration("20190214183645_AddCourseCanvasId")]
    partial class AddCourseCanvasId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CourseCleanup.Models.CourseSearchQueue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("LastUpdated");

                    b.Property<int>("Status");

                    b.Property<string>("StatusMessage");

                    b.Property<string>("SubmittedByEmail");

                    b.Property<string>("TermList");

                    b.HasKey("Id");

                    b.ToTable("CourseSearchQueue");
                });

            modelBuilder.Entity("CourseCleanup.Models.UnusedCourse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountId");

                    b.Property<string>("CourseCanvasId");

                    b.Property<string>("CourseCode");

                    b.Property<string>("CourseId");

                    b.Property<string>("CourseName");

                    b.Property<string>("CourseSISID");

                    b.Property<int>("CourseSearchQueueId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("LastUpdated");

                    b.Property<int>("Status");

                    b.Property<string>("Term");

                    b.Property<string>("TermId");

                    b.HasKey("Id");

                    b.HasIndex("CourseSearchQueueId");

                    b.ToTable("UnusedCourse");
                });

            modelBuilder.Entity("CourseCleanup.Models.UnusedCourse", b =>
                {
                    b.HasOne("CourseCleanup.Models.CourseSearchQueue", "CourseSearchQueue")
                        .WithMany()
                        .HasForeignKey("CourseSearchQueueId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
