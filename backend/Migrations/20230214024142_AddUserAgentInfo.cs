using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapMe.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAgentInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "parsedDevice",
                table: "userAgents",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "parsedOS",
                table: "userAgents",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "parsedUA",
                table: "userAgents",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "sessions",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "parsedDevice",
                table: "userAgents");

            migrationBuilder.DropColumn(
                name: "parsedOS",
                table: "userAgents");

            migrationBuilder.DropColumn(
                name: "parsedUA",
                table: "userAgents");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "sessions",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
