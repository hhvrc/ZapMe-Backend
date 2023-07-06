using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapMe.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceModelAndDeviceManufacturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ModelId",
                table: "Devices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "DeviceManufacturers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IconId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceManufacturers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceManufacturers_Images_IconId",
                        column: x => x.IconId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ModelNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IconId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FccId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    HasDocumentation = table.Column<bool>(type: "boolean", nullable: false),
                    Protocol = table.Column<string>(type: "text", nullable: false),
                    SpecificationId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceModels_DeviceManufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "DeviceManufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceModels_Images_IconId",
                        column: x => x.IconId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ModelId",
                table: "Devices",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceManufacturers_IconId",
                table: "DeviceManufacturers",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceManufacturers_Name",
                table: "DeviceManufacturers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModels_IconId",
                table: "DeviceModels",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModels_ManufacturerId",
                table: "DeviceModels",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModels_ModelNumber_ManufacturerId",
                table: "DeviceModels",
                columns: new[] { "ModelNumber", "ManufacturerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceModels_Name",
                table: "DeviceModels",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceModels_ModelId",
                table: "Devices",
                column: "ModelId",
                principalTable: "DeviceModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceModels_ModelId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "DeviceModels");

            migrationBuilder.DropTable(
                name: "DeviceManufacturers");

            migrationBuilder.DropIndex(
                name: "IX_Devices_ModelId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Devices");
        }
    }
}
