using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "art_cover_url",
                schema: "public",
                table: "t_e_article_art",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "art_description",
                schema: "public",
                table: "t_e_article_art",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "art_cover_url",
                schema: "public",
                table: "t_e_article_art");

            migrationBuilder.DropColumn(
                name: "art_description",
                schema: "public",
                table: "t_e_article_art");
        }
    }
}
