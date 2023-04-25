﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ZapMe.Data;

#nullable disable

namespace ZapMe.Migrations
{
    [DbContext(typeof(ZapMeContext))]
    partial class ZapMeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ZapMe.Data.Models.EmailVerificationRequestEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userId");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("NewEmail")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)")
                        .HasColumnName("newEmail");

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("tokenHash");

                    b.HasKey("UserId");

                    b.HasIndex("NewEmail")
                        .IsUnique()
                        .HasDatabaseName("emailVerificationRequest_newEmail_idx");

                    b.HasIndex("TokenHash")
                        .IsUnique()
                        .HasDatabaseName("emailVerificationRequest_tokenHash_idx");

                    b.ToTable("emailVerificationRequest", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.FriendRequestEntity", b =>
                {
                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid")
                        .HasColumnName("senderId");

                    b.Property<Guid>("ReceiverId")
                        .HasColumnType("uuid")
                        .HasColumnName("receiverId");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.HasKey("SenderId", "ReceiverId");

                    b.HasIndex("ReceiverId");

                    b.ToTable("friendRequests", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.ImageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)")
                        .HasColumnName("extension");

                    b.Property<long>("FrameCount")
                        .HasColumnType("bigint")
                        .HasColumnName("frameCount");

                    b.Property<long>("Height")
                        .HasColumnType("bigint")
                        .HasColumnName("height");

                    b.Property<string>("S3BucketName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("s3BucketName");

                    b.Property<string>("S3RegionName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("s3RegionName");

                    b.Property<string>("Sha256")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("sha256");

                    b.Property<long>("SizeBytes")
                        .HasColumnType("bigint")
                        .HasColumnName("sizeBytes");

                    b.Property<Guid?>("UploaderId")
                        .HasColumnType("uuid")
                        .HasColumnName("uploaderId");

                    b.Property<long>("Width")
                        .HasColumnType("bigint")
                        .HasColumnName("width");

                    b.HasKey("Id");

                    b.HasIndex("Sha256")
                        .IsUnique()
                        .HasDatabaseName("images_sha256_idx");

                    b.HasIndex("UploaderId");

                    b.ToTable("images", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.LockOutEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiresAt");

                    b.Property<string>("Flags")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("flags");

                    b.Property<string>("Reason")
                        .HasColumnType("text")
                        .HasColumnName("reason");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("lockOuts", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.OAuthConnectionEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userId");

                    b.Property<string>("ProviderName")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)")
                        .HasColumnName("providerName");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("ProviderId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("providerId");

                    b.HasKey("UserId", "ProviderName");

                    b.ToTable("oauthConnections", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.PasswordResetRequestEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userId");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("TokenHash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("tokenHash");

                    b.HasKey("UserId");

                    b.HasIndex("TokenHash")
                        .IsUnique()
                        .HasDatabaseName("passwordResetRequests_tokenHash_idx");

                    b.ToTable("passwordResetRequests", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.SessionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("character varying(2)")
                        .HasColumnName("country");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiresAt");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)")
                        .HasColumnName("ipAddress");

                    b.Property<string>("Name")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("name");

                    b.Property<Guid>("UserAgentId")
                        .HasColumnType("uuid")
                        .HasColumnName("userAgentId");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userId");

                    b.HasKey("Id");

                    b.HasIndex("UserAgentId");

                    b.HasIndex("UserId");

                    b.ToTable("sessions", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserAgentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<string>("Browser")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("browser");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Device")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("device");

                    b.Property<long>("Length")
                        .HasColumnType("bigint")
                        .HasColumnName("length");

                    b.Property<string>("OperatingSystem")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("operatingSystem");

                    b.Property<string>("Sha256")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("sha256");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.HasIndex("Sha256")
                        .IsUnique()
                        .HasDatabaseName("userAgents_hash_idx");

                    b.ToTable("userAgents", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<int>("AcceptedTosVersion")
                        .HasColumnType("integer")
                        .HasColumnName("acceptedTosVersion");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailVerified")
                        .HasColumnType("boolean")
                        .HasColumnName("emailVerified");

                    b.Property<DateTime>("LastOnline")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lastOnline")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("name");

                    b.Property<int>("OnlineStatus")
                        .HasColumnType("integer")
                        .HasColumnName("statusOnline");

                    b.Property<string>("OnlineStatusText")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("statusText");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("character varying(120)")
                        .HasColumnName("passwordHash");

                    b.Property<Guid>("ProfilePictureId")
                        .HasColumnType("uuid")
                        .HasColumnName("profilePictureId");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updatedAt")
                        .HasDefaultValueSql("now()");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("users_email_idx");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("users_name_idx");

                    b.HasIndex("ProfilePictureId");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserRelationEntity", b =>
                {
                    b.Property<Guid>("SourceUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("sourceUserId");

                    b.Property<Guid>("TargetUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("targetUserId");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("NickName")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("nickName");

                    b.Property<string>("Notes")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("notes");

                    b.Property<int>("RelationType")
                        .HasColumnType("integer")
                        .HasColumnName("relationType");

                    b.HasKey("SourceUserId", "TargetUserId");

                    b.HasIndex("TargetUserId");

                    b.ToTable("userRelations", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserRoleEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userId");

                    b.Property<string>("RoleName")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("roleName");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createdAt")
                        .HasDefaultValueSql("now()");

                    b.HasKey("UserId", "RoleName");

                    b.HasIndex("RoleName")
                        .HasDatabaseName("userRoles_roleName_idx");

                    b.HasIndex("UserId")
                        .HasDatabaseName("userRoles_userId_idx");

                    b.ToTable("userRoles", (string)null);
                });

            modelBuilder.Entity("ZapMe.Data.Models.EmailVerificationRequestEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserEntity", "User")
                        .WithOne("EmailVerificationRequest")
                        .HasForeignKey("ZapMe.Data.Models.EmailVerificationRequestEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Data.Models.FriendRequestEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserEntity", "Receiver")
                        .WithMany("FriendRequestsIncoming")
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZapMe.Data.Models.UserEntity", "Sender")
                        .WithMany("FriendRequestsOutgoing")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("ZapMe.Data.Models.ImageEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserEntity", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("ZapMe.Data.Models.LockOutEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserEntity", "User")
                        .WithMany("LockOuts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Data.Models.OAuthConnectionEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserEntity", "User")
                        .WithMany("OauthConnections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Data.Models.PasswordResetRequestEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserEntity", "User")
                        .WithOne()
                        .HasForeignKey("ZapMe.Data.Models.PasswordResetRequestEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Data.Models.SessionEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserAgentEntity", "UserAgent")
                        .WithMany("Sessions")
                        .HasForeignKey("UserAgentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZapMe.Data.Models.UserEntity", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("UserAgent");
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.ImageEntity", "ProfilePicture")
                        .WithMany()
                        .HasForeignKey("ProfilePictureId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("ProfilePicture");
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserRelationEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserEntity", "SourceUser")
                        .WithMany("Relations")
                        .HasForeignKey("SourceUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ZapMe.Data.Models.UserEntity", "TargetUser")
                        .WithMany()
                        .HasForeignKey("TargetUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SourceUser");

                    b.Navigation("TargetUser");
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserRoleEntity", b =>
                {
                    b.HasOne("ZapMe.Data.Models.UserEntity", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserAgentEntity", b =>
                {
                    b.Navigation("Sessions");
                });

            modelBuilder.Entity("ZapMe.Data.Models.UserEntity", b =>
                {
                    b.Navigation("EmailVerificationRequest");

                    b.Navigation("FriendRequestsIncoming");

                    b.Navigation("FriendRequestsOutgoing");

                    b.Navigation("LockOuts");

                    b.Navigation("OauthConnections");

                    b.Navigation("Relations");

                    b.Navigation("Sessions");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
