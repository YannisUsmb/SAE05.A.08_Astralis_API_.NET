using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class FixUserNotificationPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "usernotification_pkey",
                schema: "public",
                table: "t_j_usernotification_uno");

            migrationBuilder.RenameTable(
                name: "t_j_usernotification_uno",
                schema: "public",
                newName: "t_e_usernotification_uno",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_t_j_usernotification_uno_not_id",
                schema: "public",
                table: "t_e_usernotification_uno",
                newName: "IX_t_e_usernotification_uno_not_id");

            migrationBuilder.AddColumn<int>(
                name: "uno_id",
                schema: "public",
                table: "t_e_usernotification_uno",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "usernotification_pkey",
                schema: "public",
                table: "t_e_usernotification_uno",
                column: "uno_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_usernotification_uno_usr_id",
                schema: "public",
                table: "t_e_usernotification_uno",
                column: "usr_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "usernotification_pkey",
                schema: "public",
                table: "t_e_usernotification_uno");

            migrationBuilder.DropIndex(
                name: "IX_t_e_usernotification_uno_usr_id",
                schema: "public",
                table: "t_e_usernotification_uno");

            migrationBuilder.DropColumn(
                name: "uno_id",
                schema: "public",
                table: "t_e_usernotification_uno");

            migrationBuilder.RenameTable(
                name: "t_e_usernotification_uno",
                schema: "public",
                newName: "t_j_usernotification_uno",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_t_e_usernotification_uno_not_id",
                schema: "public",
                table: "t_j_usernotification_uno",
                newName: "IX_t_j_usernotification_uno_not_id");

            migrationBuilder.AddPrimaryKey(
                name: "usernotification_pkey",
                schema: "public",
                table: "t_j_usernotification_uno",
                columns: new[] { "usr_id", "not_id" });
        }
    }
}
