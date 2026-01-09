using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "art_date",
                schema: "public",
                table: "t_e_article_art",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "art_date",
                schema: "public",
                table: "t_e_article_art");
        }
    }
}
