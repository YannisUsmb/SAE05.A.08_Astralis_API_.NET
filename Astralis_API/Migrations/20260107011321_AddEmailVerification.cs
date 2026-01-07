using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "usr_is_verified",
                schema: "public",
                table: "t_e_user_usr",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "usr_token_expiration",
                schema: "public",
                table: "t_e_user_usr",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "usr_verification_token",
                schema: "public",
                table: "t_e_user_usr",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "usr_is_verified",
                schema: "public",
                table: "t_e_user_usr");

            migrationBuilder.DropColumn(
                name: "usr_token_expiration",
                schema: "public",
                table: "t_e_user_usr");

            migrationBuilder.DropColumn(
                name: "usr_verification_token",
                schema: "public",
                table: "t_e_user_usr");
        }
    }
}
