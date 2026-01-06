using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "php_example",
                schema: "public",
                table: "t_e_phoneprefix_php",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "php_regex",
                schema: "public",
                table: "t_e_phoneprefix_php",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "php_example",
                schema: "public",
                table: "t_e_phoneprefix_php");

            migrationBuilder.DropColumn(
                name: "php_regex",
                schema: "public",
                table: "t_e_phoneprefix_php");
        }
    }
}
