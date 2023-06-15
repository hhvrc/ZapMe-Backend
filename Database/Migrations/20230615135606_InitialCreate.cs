using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapMe.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "deletedUsers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    deletedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    deletionReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    userCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    userDeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deletedUsers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "userAgents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    length = table.Column<long>(type: "bigint", nullable: false),
                    value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    operatingSystem = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    device = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    browser = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userAgents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "emailVerificationRequest",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    newEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    tokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emailVerificationRequest", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "friendRequests",
                columns: table => new
                {
                    senderId = table.Column<Guid>(type: "uuid", nullable: false),
                    receiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendRequests", x => new { x.senderId, x.receiverId });
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    height = table.Column<long>(type: "bigint", nullable: false),
                    width = table.Column<long>(type: "bigint", nullable: false),
                    frameCount = table.Column<long>(type: "bigint", nullable: false),
                    sizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    mimeType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    r2RegionName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    uploaderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    emailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    passwordHash = table.Column<string>(type: "character varying(72)", maxLength: 72, nullable: false),
                    acceptedPrivacyPolicyVersion = table.Column<long>(type: "bigint", nullable: false),
                    acceptedTermsOfServiceVersion = table.Column<long>(type: "bigint", nullable: false),
                    profileAvatarId = table.Column<Guid>(type: "uuid", nullable: true),
                    profileBannerId = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    statusText = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastOnline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_images_profileAvatarId",
                        column: x => x.profileAvatarId,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_users_images_profileBannerId",
                        column: x => x.profileBannerId,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "lockOuts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    flags = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lockOuts", x => x.id);
                    table.ForeignKey(
                        name: "FK_lockOuts_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    nickName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ipAddress = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    userAgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_sessions_userAgents_userAgentId",
                        column: x => x.userAgentId,
                        principalTable: "userAgents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sessions_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ssoConnections",
                columns: table => new
                {
                    providerName = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    providerUserId = table.Column<string>(type: "text", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    providerUserName = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ssoConnections", x => new { x.providerName, x.providerUserId });
                    table.ForeignKey(
                        name: "FK_ssoConnections_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userPasswordResetRequests",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    tokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userPasswordResetRequests", x => x.userId);
                    table.ForeignKey(
                        name: "FK_userPasswordResetRequests_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userRelations",
                columns: table => new
                {
                    sourceUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    targetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    relationType = table.Column<int>(type: "integer", nullable: false),
                    nickName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    notes = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRelations", x => new { x.sourceUserId, x.targetUserId });
                    table.ForeignKey(
                        name: "FK_userRelations_users_sourceUserId",
                        column: x => x.sourceUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userRelations_users_targetUserId",
                        column: x => x.targetUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userRoles",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    roleName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRoles", x => new { x.userId, x.roleName });
                    table.ForeignKey(
                        name: "FK_userRoles_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "emailVerificationRequest_newEmail_idx",
                table: "emailVerificationRequest",
                column: "newEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "emailVerificationRequest_tokenHash_idx",
                table: "emailVerificationRequest",
                column: "tokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_friendRequests_receiverId",
                table: "friendRequests",
                column: "receiverId");

            migrationBuilder.CreateIndex(
                name: "images_sha256_idx",
                table: "images",
                column: "sha256",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_images_uploaderId",
                table: "images",
                column: "uploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_lockOuts_userId",
                table: "lockOuts",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_userAgentId",
                table: "sessions",
                column: "userAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_userId",
                table: "sessions",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_ssoConnections_userId",
                table: "ssoConnections",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "userAgents_hash_idx",
                table: "userAgents",
                column: "sha256",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "userPasswordResetRequests_tokenHash_idx",
                table: "userPasswordResetRequests",
                column: "tokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userRelations_targetUserId",
                table: "userRelations",
                column: "targetUserId");

            migrationBuilder.CreateIndex(
                name: "userRoles_roleName_idx",
                table: "userRoles",
                column: "roleName");

            migrationBuilder.CreateIndex(
                name: "userRoles_userId_idx",
                table: "userRoles",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_users_profileAvatarId",
                table: "users",
                column: "profileAvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_users_profileBannerId",
                table: "users",
                column: "profileBannerId");

            migrationBuilder.CreateIndex(
                name: "users_email_idx",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_name_idx",
                table: "users",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_emailVerificationRequest_users_userId",
                table: "emailVerificationRequest",
                column: "userId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_friendRequests_users_receiverId",
                table: "friendRequests",
                column: "receiverId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_friendRequests_users_senderId",
                table: "friendRequests",
                column: "senderId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_images_users_uploaderId",
                table: "images",
                column: "uploaderId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_users_uploaderId",
                table: "images");

            migrationBuilder.DropTable(
                name: "deletedUsers");

            migrationBuilder.DropTable(
                name: "emailVerificationRequest");

            migrationBuilder.DropTable(
                name: "friendRequests");

            migrationBuilder.DropTable(
                name: "lockOuts");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "ssoConnections");

            migrationBuilder.DropTable(
                name: "userPasswordResetRequests");

            migrationBuilder.DropTable(
                name: "userRelations");

            migrationBuilder.DropTable(
                name: "userRoles");

            migrationBuilder.DropTable(
                name: "userAgents");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "images");
        }
    }
}
