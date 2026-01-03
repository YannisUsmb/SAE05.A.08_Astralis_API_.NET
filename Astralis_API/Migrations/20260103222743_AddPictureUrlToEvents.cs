using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class AddPictureUrlToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "eve_pictureurl",
                schema: "public",
                table: "t_e_event_eve",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "eve_pictureurl",
                schema: "public",
                table: "t_e_event_eve");
        }
    }
}
