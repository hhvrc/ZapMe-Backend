using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapMe.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateDeviceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Images_IconId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Devices",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<Guid>(
                name: "IconId",
                table: "Devices",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "Devices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Devices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Devices_AccessToken",
                table: "Devices",
                column: "AccessToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_OwnerId",
                table: "Devices",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Images_IconId",
                table: "Devices",
                column: "IconId",
                principalTable: "Images",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Users_OwnerId",
                table: "Devices",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Images_IconId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Users_OwnerId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_AccessToken",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_OwnerId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Devices");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Devices",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<Guid>(
                name: "IconId",
                table: "Devices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    IconId = table.Column<Guid>(type: "uuid", nullable: true),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
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
    }
}
