﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ZapMe.Database;

#nullable disable

namespace ZapMe.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ZapMe.Database.Models.DeletedUserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("DeletionReason")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("UserCreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UserDeletedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.HasKey("Id");

                    b.ToTable("DeletedUsers");
                });

            modelBuilder.Entity("ZapMe.Database.Models.DeviceEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.HasIndex("ModelId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("ZapMe.Database.Models.DeviceManufacturerEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("IconId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("WebsiteUrl")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("IconId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("DeviceManufacturers");
                });

            modelBuilder.Entity("ZapMe.Database.Models.DeviceModelEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("FccId")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<bool>("HasDocumentation")
                        .HasColumnType("boolean");

                    b.Property<Guid>("IconId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ManufacturerId")
                        .HasColumnType("uuid");

                    b.Property<string>("ModelNumber")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Protocol")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("SpecificationId")
                        .HasColumnType("uuid");

                    b.Property<string>("WebsiteUrl")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("IconId");

                    b.HasIndex("ManufacturerId");

                    b.HasIndex("Name");

                    b.HasIndex("ModelNumber", "ManufacturerId")
                        .IsUnique();

                    b.ToTable("DeviceModels");
                });

            modelBuilder.Entity("ZapMe.Database.Models.ImageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<long>("FrameCount")
                        .HasColumnType("bigint");

                    b.Property<long>("Height")
                        .HasColumnType("bigint");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("R2RegionName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Sha256")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<long>("SizeBytes")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("UploaderId")
                        .HasColumnType("uuid");

                    b.Property<long>("Width")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Sha256")
                        .IsUnique();

                    b.HasIndex("UploaderId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("ZapMe.Database.Models.LockOutEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Flags")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("LockOuts");
                });

            modelBuilder.Entity("ZapMe.Database.Models.PrivacyPolicyDocumentEntity", b =>
                {
                    b.Property<long>("Version")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Version"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Markdown")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Version");

                    b.ToTable("PrivacyPolicyDocuments");
                });

            modelBuilder.Entity("ZapMe.Database.Models.SSOConnectionEntity", b =>
                {
                    b.Property<string>("ProviderName")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("ProviderUserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("ProviderUserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("ProviderName", "ProviderUserId");

                    b.HasIndex("UserId");

                    b.ToTable("SSOConnections");
                });

            modelBuilder.Entity("ZapMe.Database.Models.SessionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("character varying(2)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("NickName")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<Guid>("UserAgentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserAgentId");

                    b.HasIndex("UserId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("ZapMe.Database.Models.TermsOfServiceDocumentEntity", b =>
                {
                    b.Property<long>("Version")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Version"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Markdown")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Version");

                    b.ToTable("TermsOfServiceDocuments");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserAgentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("Browser")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Device")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<long>("Length")
                        .HasColumnType("bigint");

                    b.Property<string>("OperatingSystem")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Sha256")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.HasKey("Id");

                    b.HasIndex("Sha256")
                        .IsUnique();

                    b.ToTable("UserAgents");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserEmailVerificationRequestEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("NewEmail")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)");

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("UserId");

                    b.HasIndex("NewEmail")
                        .IsUnique();

                    b.HasIndex("TokenHash")
                        .IsUnique();

                    b.ToTable("UserEmailVerificationRequests");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<long>("AcceptedPrivacyPolicyVersion")
                        .HasColumnType("bigint");

                    b.Property<long>("AcceptedTermsOfServiceVersion")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)");

                    b.Property<bool>("EmailVerified")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastOnline")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(72)
                        .HasColumnType("character varying(72)");

                    b.Property<Guid?>("ProfileAvatarId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ProfileBannerId")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("StatusText")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ProfileAvatarId");

                    b.HasIndex("ProfileBannerId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserPasswordResetRequestEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("UserId");

                    b.HasIndex("TokenHash")
                        .IsUnique();

                    b.ToTable("UserPasswordResetRequests");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserRelationEntity", b =>
                {
                    b.Property<Guid>("FromUserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ToUserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<int>("FriendStatus")
                        .HasColumnType("integer");

                    b.Property<bool>("IsFavorite")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMuted")
                        .HasColumnType("boolean");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("FromUserId", "ToUserId");

                    b.HasIndex("ToUserId");

                    b.ToTable("UserRelations");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserRoleEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("RoleName")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.HasKey("UserId", "RoleName");

                    b.HasIndex("RoleName");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("ZapMe.Database.Models.DeviceEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.DeviceModelEntity", "Model")
                        .WithMany()
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Model");
                });

            modelBuilder.Entity("ZapMe.Database.Models.DeviceManufacturerEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.ImageEntity", "Icon")
                        .WithMany()
                        .HasForeignKey("IconId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Icon");
                });

            modelBuilder.Entity("ZapMe.Database.Models.DeviceModelEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.ImageEntity", "Icon")
                        .WithMany()
                        .HasForeignKey("IconId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZapMe.Database.Models.DeviceManufacturerEntity", "Manufacturer")
                        .WithMany()
                        .HasForeignKey("ManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Icon");

                    b.Navigation("Manufacturer");
                });

            modelBuilder.Entity("ZapMe.Database.Models.ImageEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.UserEntity", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("ZapMe.Database.Models.LockOutEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.UserEntity", "User")
                        .WithMany("LockOuts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Database.Models.SSOConnectionEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.UserEntity", "User")
                        .WithMany("SSOConnections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Database.Models.SessionEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.UserAgentEntity", "UserAgent")
                        .WithMany("Sessions")
                        .HasForeignKey("UserAgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZapMe.Database.Models.UserEntity", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("UserAgent");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserEmailVerificationRequestEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.UserEntity", "User")
                        .WithOne("EmailVerificationRequest")
                        .HasForeignKey("ZapMe.Database.Models.UserEmailVerificationRequestEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.ImageEntity", "ProfileAvatar")
                        .WithMany()
                        .HasForeignKey("ProfileAvatarId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("ZapMe.Database.Models.ImageEntity", "ProfileBanner")
                        .WithMany()
                        .HasForeignKey("ProfileBannerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("ProfileAvatar");

                    b.Navigation("ProfileBanner");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserPasswordResetRequestEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.UserEntity", "User")
                        .WithOne("PasswordResetRequest")
                        .HasForeignKey("ZapMe.Database.Models.UserPasswordResetRequestEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserRelationEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.UserEntity", "SourceUser")
                        .WithMany("RelationsOutgoing")
                        .HasForeignKey("FromUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZapMe.Database.Models.UserEntity", "TargetUser")
                        .WithMany("RelationsIncoming")
                        .HasForeignKey("ToUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SourceUser");

                    b.Navigation("TargetUser");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserRoleEntity", b =>
                {
                    b.HasOne("ZapMe.Database.Models.UserEntity", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserAgentEntity", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("ZapMe.Database.Models.UserEntity", b =>
                {
                    b.Navigation("EmailVerificationRequest");

                    b.Navigation("LockOuts");

                    b.Navigation("PasswordResetRequest");

                    b.Navigation("RelationsIncoming");

                    b.Navigation("RelationsOutgoing");

                    b.Navigation("SSOConnections");

                    b.Navigation("Sessions");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
