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
                name: "emailTemplates",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emailTemplates", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "userAgents",
                columns: table => new
                {
                    hash = table.Column<byte[]>(type: "bytea", maxLength: 32, nullable: false),
                    length = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    parsedOS = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    parsedDevice = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    parsedUA = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userAgents", x => x.hash);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    emailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    passwordHash = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    acceptedTosVersion = table.Column<int>(type: "integer", nullable: false),
                    profilePictureId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_accounts", x => x.id);
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
                    table.ForeignKey(
                        name: "FK_friendRequests_accounts_receiverId",
                        column: x => x.receiverId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_friendRequests_accounts_senderId",
                        column: x => x.senderId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    height = table.Column<int>(type: "integer", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    sizeBytes = table.Column<int>(type: "integer", nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    phash = table.Column<long>(type: "bigint", nullable: false),
                    uploaderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_images_accounts_uploaderId",
                        column: x => x.uploaderId,
                        principalTable: "accounts",
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
                        name: "FK_lockOuts_accounts_userId",
                        column: x => x.userId,
                        principalTable: "accounts",
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
                        name: "FK_oauthConnections_accounts_userId",
                        column: x => x.userId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ipAddress = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    userAgent = table.Column<byte[]>(type: "bytea", maxLength: 32, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_sessions_accounts_userId",
                        column: x => x.userId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sessions_userAgents_userAgent",
                        column: x => x.userAgent,
                        principalTable: "userAgents",
                        principalColumn: "hash",
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
                        name: "FK_userRelations_accounts_sourceUserId",
                        column: x => x.sourceUserId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userRelations_accounts_targetUserId",
                        column: x => x.targetUserId,
                        principalTable: "accounts",
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
                        name: "FK_userRoles_accounts_userId",
                        column: x => x.userId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "accounts_email_idx",
                table: "accounts",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "accounts_name_idx",
                table: "accounts",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "accounts_passwordResetToken_idx",
                table: "accounts",
                column: "passwordResetToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_accounts_profilePictureId",
                table: "accounts",
                column: "profilePictureId");

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
                name: "IX_sessions_userAgent",
                table: "sessions",
                column: "userAgent");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_userId",
                table: "sessions",
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

            migrationBuilder.AddForeignKey(
                name: "FK_accounts_images_profilePictureId",
                table: "accounts",
                column: "profilePictureId",
                principalTable: "images",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_accounts_images_profilePictureId",
                table: "accounts");

            migrationBuilder.DropTable(
                name: "emailTemplates");

            migrationBuilder.DropTable(
                name: "friendRequests");

            migrationBuilder.DropTable(
                name: "lockOuts");

            migrationBuilder.DropTable(
                name: "oauthConnections");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "userRelations");

            migrationBuilder.DropTable(
                name: "userRoles");

            migrationBuilder.DropTable(
                name: "userAgents");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropSequence(
                name: "EntityFrameworkHiLoSequence");
        }
    }
}
