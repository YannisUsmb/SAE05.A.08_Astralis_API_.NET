using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsAndFixesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "uno_bymail",
                schema: "public",
                table: "t_j_usernotification_uno");

            migrationBuilder.AlterColumn<string>(
                name: "usr_password",
                schema: "public",
                table: "t_e_user_usr",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "CHAR(256)");

            migrationBuilder.AlterColumn<string>(
                name: "usr_avatarurl",
                schema: "public",
                table: "t_e_user_usr",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "nty_id",
                schema: "public",
                table: "t_e_notification_not",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "t_e_notification_nty",
                schema: "public",
                columns: table => new
                {
                    nty_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nty_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nty_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notificationtype_pkey", x => x.nty_id);
                });

            migrationBuilder.CreateTable(
                name: "t_j_usernotificationtype_unt",
                schema: "public",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    nty_id = table.Column<int>(type: "integer", nullable: false),
                    unt_bymail = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usernotificationtype_pkey", x => new { x.usr_id, x.nty_id });
                    table.ForeignKey(
                        name: "fk_usernotificationtype_notificationtype",
                        column: x => x.nty_id,
                        principalSchema: "public",
                        principalTable: "t_e_notification_nty",
                        principalColumn: "nty_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usernotificationtype_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_not_nty_id",
                schema: "public",
                table: "t_e_notification_not",
                column: "nty_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_usernotificationtype_unt_nty_id",
                schema: "public",
                table: "t_j_usernotificationtype_unt",
                column: "nty_id");

            migrationBuilder.AddForeignKey(
                name: "fk_notification_notificationtype",
                schema: "public",
                table: "t_e_notification_not",
                column: "nty_id",
                principalSchema: "public",
                principalTable: "t_e_notification_nty",
                principalColumn: "nty_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_notification_notificationtype",
                schema: "public",
                table: "t_e_notification_not");

            migrationBuilder.DropTable(
                name: "t_j_usernotificationtype_unt",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_notification_nty",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_t_e_notification_not_nty_id",
                schema: "public",
                table: "t_e_notification_not");

            migrationBuilder.DropColumn(
                name: "nty_id",
                schema: "public",
                table: "t_e_notification_not");

            migrationBuilder.AddColumn<bool>(
                name: "uno_bymail",
                schema: "public",
                table: "t_j_usernotification_uno",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "usr_password",
                schema: "public",
                table: "t_e_user_usr",
                type: "CHAR(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "usr_avatarurl",
                schema: "public",
                table: "t_e_user_usr",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
