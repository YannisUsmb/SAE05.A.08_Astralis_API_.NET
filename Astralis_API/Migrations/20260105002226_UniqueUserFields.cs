using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class UniqueUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "uq_user_email",
                schema: "public",
                table: "t_e_user_usr",
                column: "usr_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_user_phone",
                schema: "public",
                table: "t_e_user_usr",
                column: "usr_phone",
                unique: true,
                filter: "usr_phone IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "uq_user_username",
                schema: "public",
                table: "t_e_user_usr",
                column: "usr_username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_user_email",
                schema: "public",
                table: "t_e_user_usr");

            migrationBuilder.DropIndex(
                name: "uq_user_phone",
                schema: "public",
                table: "t_e_user_usr");

            migrationBuilder.DropIndex(
                name: "uq_user_username",
                schema: "public",
                table: "t_e_user_usr");
        }
    }
}
