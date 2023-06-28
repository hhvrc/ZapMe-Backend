using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapMe.Database.Migrations
{
    /// <inheritdoc />
    public partial class MakeLegalTextMarkdown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "TermsOfServiceDocuments",
                newName: "Markdown");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "PrivacyPolicyDocuments",
                newName: "Markdown");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Markdown",
                table: "TermsOfServiceDocuments",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "Markdown",
                table: "PrivacyPolicyDocuments",
                newName: "Text");
        }
    }
}
