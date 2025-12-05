using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class FixedPlanetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_t_e_star_sta_ceb_id",
                schema: "public",
                table: "t_e_star_sta",
                newName: "uq_star_cebid");

            migrationBuilder.RenameIndex(
                name: "IX_t_e_satellite_sat_ceb_id",
                schema: "public",
                table: "t_e_satellite_sat",
                newName: "uq_satellite_cebid");

            migrationBuilder.RenameIndex(
                name: "IX_t_e_planet_pla_ceb_id",
                schema: "public",
                table: "t_e_planet_pla",
                newName: "uq_planet_cebid");

            migrationBuilder.RenameIndex(
                name: "IX_t_e_galaxyquasar_gaq_ceb_id",
                schema: "public",
                table: "t_e_galaxyquasar_gaq",
                newName: "uq_galaxyquasar_cebid");

            migrationBuilder.RenameIndex(
                name: "IX_t_e_comet_cmt_ceb_id",
                schema: "public",
                table: "t_e_comet_cmt",
                newName: "uq_comet_cebid");

            migrationBuilder.RenameIndex(
                name: "IX_t_e_asteroid_ast_ceb_id",
                schema: "public",
                table: "t_e_asteroid_ast",
                newName: "uq_asteroid_cebid");

            migrationBuilder.AlterColumn<string>(
                name: "sta_designation",
                schema: "public",
                table: "t_e_star_sta",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "pla_orbitalperiod",
                schema: "public",
                table: "t_e_planet_pla",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,12)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "dem_label",
                schema: "public",
                table: "t_e_detectionmethod_dem",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "uq_star_cebid",
                schema: "public",
                table: "t_e_star_sta",
                newName: "IX_t_e_star_sta_ceb_id");

            migrationBuilder.RenameIndex(
                name: "uq_satellite_cebid",
                schema: "public",
                table: "t_e_satellite_sat",
                newName: "IX_t_e_satellite_sat_ceb_id");

            migrationBuilder.RenameIndex(
                name: "uq_planet_cebid",
                schema: "public",
                table: "t_e_planet_pla",
                newName: "IX_t_e_planet_pla_ceb_id");

            migrationBuilder.RenameIndex(
                name: "uq_galaxyquasar_cebid",
                schema: "public",
                table: "t_e_galaxyquasar_gaq",
                newName: "IX_t_e_galaxyquasar_gaq_ceb_id");

            migrationBuilder.RenameIndex(
                name: "uq_comet_cebid",
                schema: "public",
                table: "t_e_comet_cmt",
                newName: "IX_t_e_comet_cmt_ceb_id");

            migrationBuilder.RenameIndex(
                name: "uq_asteroid_cebid",
                schema: "public",
                table: "t_e_asteroid_ast",
                newName: "IX_t_e_asteroid_ast_ceb_id");

            migrationBuilder.AlterColumn<string>(
                name: "sta_designation",
                schema: "public",
                table: "t_e_star_sta",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "pla_orbitalperiod",
                schema: "public",
                table: "t_e_planet_pla",
                type: "numeric(14,12)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "dem_label",
                schema: "public",
                table: "t_e_detectionmethod_dem",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}
