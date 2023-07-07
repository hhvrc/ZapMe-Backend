using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapMe.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceAndControlGrant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IconId",
                table: "Devices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ControlGrants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlGrants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControlGrants_Users_SubjectUserId",
                        column: x => x.SubjectUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IconId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => new { x.UserId, x.DeviceId });
                    table.ForeignKey(
                        name: "FK_UserDevices_Images_IconId",
                        column: x => x.IconId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ControlGrantPublicShares",
                columns: table => new
                {
                    GrantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessKey = table.Column<string>(type: "text", nullable: false),
                    UsesLeft = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlGrantPublicShares", x => new { x.GrantId, x.AccessKey });
                    table.ForeignKey(
                        name: "FK_ControlGrantPublicShares_ControlGrants_GrantId",
                        column: x => x.GrantId,
                        principalTable: "ControlGrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ControlGrantUserShares",
                columns: table => new
                {
                    GrantId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrantedUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsesLeft = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlGrantUserShares", x => new { x.GrantId, x.GrantedUserId });
                    table.ForeignKey(
                        name: "FK_ControlGrantUserShares_ControlGrants_GrantId",
                        column: x => x.GrantId,
                        principalTable: "ControlGrants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ControlGrantUserShares_Users_GrantedUserId",
                        column: x => x.GrantedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_IconId",
                table: "Devices",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlGrantPublicShares_AccessKey",
                table: "ControlGrantPublicShares",
                column: "AccessKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ControlGrantPublicShares_GrantId",
                table: "ControlGrantPublicShares",
                column: "GrantId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlGrants_SubjectUserId",
                table: "ControlGrants",
                column: "SubjectUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlGrantUserShares_GrantedUserId",
                table: "ControlGrantUserShares",
                column: "GrantedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_DeviceId",
                table: "UserDevices",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_IconId",
                table: "UserDevices",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Images_IconId",
                table: "Devices",
                column: "IconId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Images_IconId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "ControlGrantPublicShares");

            migrationBuilder.DropTable(
                name: "ControlGrantUserShares");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.DropTable(
                name: "ControlGrants");

            migrationBuilder.DropIndex(
                name: "IX_Devices_IconId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "IconId",
                table: "Devices");
        }
    }
}
