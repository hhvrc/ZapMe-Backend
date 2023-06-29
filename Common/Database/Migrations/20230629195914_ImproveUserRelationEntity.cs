using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZapMe.Database.Migrations
{
    /// <inheritdoc />
    public partial class ImproveUserRelationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRelations_Users_SourceUserId",
                table: "UserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRelations_Users_TargetUserId",
                table: "UserRelations");

            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.RenameColumn(
                name: "RelationType",
                table: "UserRelations",
                newName: "FriendStatus");

            migrationBuilder.RenameColumn(
                name: "TargetUserId",
                table: "UserRelations",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "SourceUserId",
                table: "UserRelations",
                newName: "FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRelations_TargetUserId",
                table: "UserRelations",
                newName: "IX_UserRelations_ToUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "UserRelations",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NickName",
                table: "UserRelations",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "UserRelations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMuted",
                table: "UserRelations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelations_Users_FromUserId",
                table: "UserRelations",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelations_Users_ToUserId",
                table: "UserRelations",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRelations_Users_FromUserId",
                table: "UserRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRelations_Users_ToUserId",
                table: "UserRelations");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "UserRelations");

            migrationBuilder.DropColumn(
                name: "IsMuted",
                table: "UserRelations");

            migrationBuilder.RenameColumn(
                name: "FriendStatus",
                table: "UserRelations",
                newName: "RelationType");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "UserRelations",
                newName: "TargetUserId");

            migrationBuilder.RenameColumn(
                name: "FromUserId",
                table: "UserRelations",
                newName: "SourceUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRelations_ToUserId",
                table: "UserRelations",
                newName: "IX_UserRelations_TargetUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "UserRelations",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "NickName",
                table: "UserRelations",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => new { x.SenderId, x.ReceiverId });
                    table.ForeignKey(
                        name: "FK_FriendRequests_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendRequests_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelations_Users_SourceUserId",
                table: "UserRelations",
                column: "SourceUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRelations_Users_TargetUserId",
                table: "UserRelations",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
