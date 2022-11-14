using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapMe.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "EntityFrameworkHiLoSequence",
                incrementBy: 10);

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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    sizeBytes = table.Column<int>(type: "integer", nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    phash = table.Column<long>(type: "bigint", nullable: false),
                    uploaderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    emailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    passwordHash = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    acceptedTosVersion = table.Column<int>(type: "integer", nullable: false),
                    profilePictureId = table.Column<Guid>(type: "uuid", nullable: true),
                    statusOnline = table.Column<int>(type: "integer", nullable: false),
                    statusText = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    passwordResetToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    passwordResetRequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    lastOnline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_images_profilePictureId",
                        column: x => x.profilePictureId,
                        principalTable: "images",
                        principalColumn: "Id",
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
                name: "oauthConnections",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    providerName = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    providerId = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauthConnections", x => new { x.userId, x.providerName });
                    table.ForeignKey(
                        name: "FK_oauthConnections_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "signIns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    deviceName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signIns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_signIns_users_userId",
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
                name: "IX_friendRequests_receiverId",
                table: "friendRequests",
                column: "receiverId");

            migrationBuilder.CreateIndex(
                name: "IX_images_uploaderId",
                table: "images",
                column: "uploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_lockOuts_userId",
                table: "lockOuts",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_signIns_userId",
                table: "signIns",
                column: "userId");

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
                name: "IX_users_profilePictureId",
                table: "users",
                column: "profilePictureId");

            migrationBuilder.CreateIndex(
                name: "users_email_idx",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_passwordResetToken_idx",
                table: "users",
                column: "passwordResetToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_username_idx",
                table: "users",
                column: "userName",
                unique: true);

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
                name: "friendRequests");

            migrationBuilder.DropTable(
                name: "lockOuts");

            migrationBuilder.DropTable(
                name: "oauthConnections");

            migrationBuilder.DropTable(
                name: "signIns");

            migrationBuilder.DropTable(
                name: "userRelations");

            migrationBuilder.DropTable(
                name: "userRoles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropSequence(
                name: "EntityFrameworkHiLoSequence");
        }
    }
}
