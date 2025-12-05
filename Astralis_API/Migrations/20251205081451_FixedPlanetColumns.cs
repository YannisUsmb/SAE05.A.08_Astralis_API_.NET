using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Astralis_API.Migrations
{
    /// <inheritdoc />
    public partial class FixedPlanetColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "t_e_aliasstatus_als",
                schema: "public",
                columns: table => new
                {
                    als_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    als_label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("aliasstatus_pkey", x => x.als_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_articletype_aty",
                schema: "public",
                columns: table => new
                {
                    aty_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    aty_label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aty_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("articletype_pkey", x => x.aty_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_celestialbodytype_cbt",
                schema: "public",
                columns: table => new
                {
                    cbt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cbt_label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cbt_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("celestialbodytype_pkey", x => x.cbt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_commandstatus_cos",
                schema: "public",
                columns: table => new
                {
                    cos_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cos_label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("commandstatus_pkey", x => x.cos_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_detectionmethod_dem",
                schema: "public",
                columns: table => new
                {
                    dem_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dem_label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    dem_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("detectionmethod_pkey", x => x.dem_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_discoverystatus_dst",
                schema: "public",
                columns: table => new
                {
                    dst_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dst_label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("discoverystatus_pkey", x => x.dst_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_eventtype_evt",
                schema: "public",
                columns: table => new
                {
                    evt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    evt_label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    evt_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("eventtype_pkey", x => x.evt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_galaxyquasarclass_gqc",
                schema: "public",
                columns: table => new
                {
                    gqc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gqc_label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    gqc_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("galaxyquasarclass_pkey", x => x.gqc_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notificationtype_nty",
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
                name: "t_e_orbitalclass_oct",
                schema: "public",
                columns: table => new
                {
                    oct_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    oct_label = table.Column<string>(type: "CHAR(3)", nullable: false),
                    oct_description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orbitalclass_pkey", x => x.oct_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_phoneprefix_php",
                schema: "public",
                columns: table => new
                {
                    php_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    php_label = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("phoneprefix_pkey", x => x.php_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_planettype_plt",
                schema: "public",
                columns: table => new
                {
                    plt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plt_label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    plt_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("planettype_pkey", x => x.plt_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_productcategory_prc",
                schema: "public",
                columns: table => new
                {
                    prc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prc_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    prc_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("productcategory_pkey", x => x.prc_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_reportmotive_rem",
                schema: "public",
                columns: table => new
                {
                    rem_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rem_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    rem_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reportmotive_pkey", x => x.rem_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_reportstatus_rst",
                schema: "public",
                columns: table => new
                {
                    rst_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rst_label = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reportstatus_pkey", x => x.rst_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_spectralclass_spc",
                schema: "public",
                columns: table => new
                {
                    spc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    spc_label = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    spc_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("spectralclass_pkey", x => x.spc_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_userrole_uro",
                schema: "public",
                columns: table => new
                {
                    uro_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uro_label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("userrole_pkey", x => x.uro_id);
                });

            migrationBuilder.CreateTable(
                name: "t_e_audio_aud",
                schema: "public",
                columns: table => new
                {
                    aud_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cbt_id = table.Column<int>(type: "integer", nullable: false),
                    aud_title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    aud_decription = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    aud_filepath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("audio_pkey", x => x.aud_id);
                    table.ForeignKey(
                        name: "fk_audio_celestialbodytype",
                        column: x => x.cbt_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbodytype_cbt",
                        principalColumn: "cbt_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_celestialbody_ceb",
                schema: "public",
                columns: table => new
                {
                    ceb_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cbt_id = table.Column<int>(type: "integer", nullable: false),
                    ceb_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ceb_alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("celestialbody_pkey", x => x.ceb_id);
                    table.ForeignKey(
                        name: "fk_celestialbody_celestialbodytype",
                        column: x => x.cbt_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbodytype_cbt",
                        principalColumn: "cbt_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_notification_not",
                schema: "public",
                columns: table => new
                {
                    not_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nty_id = table.Column<int>(type: "integer", nullable: false),
                    not_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    not_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notification_pkey", x => x.not_id);
                    table.ForeignKey(
                        name: "fk_notification_notificationtype",
                        column: x => x.nty_id,
                        principalSchema: "public",
                        principalTable: "t_e_notificationtype_nty",
                        principalColumn: "nty_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_country_cou",
                schema: "public",
                columns: table => new
                {
                    cou_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    php_id = table.Column<int>(type: "integer", nullable: false),
                    cou_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("country_pkey", x => x.cou_id);
                    table.ForeignKey(
                        name: "fk_country_phoneprefix",
                        column: x => x.php_id,
                        principalSchema: "public",
                        principalTable: "t_e_phoneprefix_php",
                        principalColumn: "php_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_asteroid_ast",
                schema: "public",
                columns: table => new
                {
                    ast_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ceb_id = table.Column<int>(type: "integer", nullable: false),
                    oct_id = table.Column<int>(type: "integer", nullable: false),
                    ast_reference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ast_absolutemagnitude = table.Column<decimal>(type: "numeric(4,2)", nullable: true),
                    ast_diameterminkm = table.Column<decimal>(type: "numeric(14,12)", nullable: true),
                    ast_diametermaxkm = table.Column<decimal>(type: "numeric(14,12)", nullable: true),
                    ast_ispotentiallyhazardous = table.Column<bool>(type: "boolean", nullable: true),
                    ast_orbitid = table.Column<int>(type: "integer", nullable: true),
                    ast_orbitdeterminationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ast_firstobservationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ast_lastobservationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ast_semimajoraxis = table.Column<decimal>(type: "numeric(14,12)", nullable: true),
                    ast_inclination = table.Column<decimal>(type: "numeric(15,12)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("asteroid_pkey", x => x.ast_id);
                    table.ForeignKey(
                        name: "fk_asteroid_celestialbody",
                        column: x => x.ceb_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbody_ceb",
                        principalColumn: "ceb_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asteroid_orbitalclass",
                        column: x => x.oct_id,
                        principalSchema: "public",
                        principalTable: "t_e_orbitalclass_oct",
                        principalColumn: "oct_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_comet_cmt",
                schema: "public",
                columns: table => new
                {
                    cmt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ceb_id = table.Column<int>(type: "integer", nullable: false),
                    cmt_orbitaleccentricity = table.Column<decimal>(type: "numeric(6,5)", nullable: true),
                    cmt_orbitalinclinationdeg = table.Column<decimal>(type: "numeric(7,4)", nullable: true),
                    cmt_ascendingnodelongitudedeg = table.Column<decimal>(type: "numeric(7,4)", nullable: true),
                    cmt_periheliondistanceau = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    cmt_apheliondistanceau = table.Column<decimal>(type: "numeric(9,3)", nullable: true),
                    cmt_orbitalperiodyears = table.Column<decimal>(type: "numeric(10,4)", nullable: true),
                    cmt_moidau = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    cmt_ref = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("comet_pkey", x => x.cmt_id);
                    table.ForeignKey(
                        name: "fk_comet_celestialbody",
                        column: x => x.ceb_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbody_ceb",
                        principalColumn: "ceb_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_galaxyquasar_gaq",
                schema: "public",
                columns: table => new
                {
                    gaq_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ceb_id = table.Column<int>(type: "integer", nullable: false),
                    gqc_id = table.Column<int>(type: "integer", nullable: false),
                    gaq_reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gaq_rightascension = table.Column<decimal>(type: "numeric(10,6)", nullable: true),
                    gaq_declination = table.Column<decimal>(type: "numeric(10,6)", nullable: true),
                    gaq_redshift = table.Column<decimal>(type: "numeric(8,6)", nullable: true),
                    gaq_rmagnitude = table.Column<decimal>(type: "numeric(8,3)", nullable: true),
                    gaq_mjdobs = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("galaxyquasar_pkey", x => x.gaq_id);
                    table.ForeignKey(
                        name: "fk_galaxyquasar_celestialbody",
                        column: x => x.ceb_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbody_ceb",
                        principalColumn: "ceb_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_galaxyquasar_galaxyquasarclass",
                        column: x => x.gqc_id,
                        principalSchema: "public",
                        principalTable: "t_e_galaxyquasarclass_gqc",
                        principalColumn: "gqc_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_planet_pla",
                schema: "public",
                columns: table => new
                {
                    pla_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ceb_id = table.Column<int>(type: "integer", nullable: false),
                    plt_id = table.Column<int>(type: "integer", nullable: false),
                    dem_id = table.Column<int>(type: "integer", nullable: false),
                    pla_distance = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    pla_discoveryyear = table.Column<int>(type: "integer", nullable: true),
                    pla_mass = table.Column<decimal>(type: "numeric(20,10)", nullable: true),
                    pla_radius = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    pla_temperature = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    pla_orbitalperiod = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    pla_eccentricity = table.Column<decimal>(type: "numeric(4,3)", nullable: true),
                    pla_stellarmagnitude = table.Column<decimal>(type: "numeric(5,3)", nullable: true),
                    pla_hoststartemperature = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    pla_hoststarmass = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    pla_remark = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("planet_pkey", x => x.pla_id);
                    table.ForeignKey(
                        name: "fk_planet_celestialbody",
                        column: x => x.ceb_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbody_ceb",
                        principalColumn: "ceb_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_planet_detectionmethod",
                        column: x => x.dem_id,
                        principalSchema: "public",
                        principalTable: "t_e_detectionmethod_dem",
                        principalColumn: "dem_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_planet_planettype",
                        column: x => x.plt_id,
                        principalSchema: "public",
                        principalTable: "t_e_planettype_plt",
                        principalColumn: "plt_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_star_sta",
                schema: "public",
                columns: table => new
                {
                    sta_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ceb_id = table.Column<int>(type: "integer", nullable: false),
                    spc_id = table.Column<int>(type: "integer", nullable: true),
                    sta_designation = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    sta_approvaldate = table.Column<DateOnly>(type: "date", nullable: true),
                    sta_constellation = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    sta_bayerdesignation = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    sta_distance = table.Column<decimal>(type: "numeric(20,15)", nullable: true),
                    sta_luminosity = table.Column<decimal>(type: "numeric(30,20)", nullable: true),
                    sta_radius = table.Column<decimal>(type: "numeric(20,15)", nullable: true),
                    sta_temperature = table.Column<decimal>(type: "numeric(20,15)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("star_pkey", x => x.sta_id);
                    table.ForeignKey(
                        name: "fk_star_celestialbody",
                        column: x => x.ceb_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbody_ceb",
                        principalColumn: "ceb_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_star_spectralclass",
                        column: x => x.spc_id,
                        principalSchema: "public",
                        principalTable: "t_e_spectralclass_spc",
                        principalColumn: "spc_id");
                });

            migrationBuilder.CreateTable(
                name: "t_e_city_cit",
                schema: "public",
                columns: table => new
                {
                    cit_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cou_id = table.Column<int>(type: "integer", nullable: false),
                    cit_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cit_postcode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cit_latitude = table.Column<decimal>(type: "numeric(10,7)", nullable: false),
                    cit_longitude = table.Column<decimal>(type: "numeric(10,7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("city_pkey", x => x.cit_id);
                    table.ForeignKey(
                        name: "fk_city_country",
                        column: x => x.cou_id,
                        principalSchema: "public",
                        principalTable: "t_e_country_cou",
                        principalColumn: "cou_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_satellite_sat",
                schema: "public",
                columns: table => new
                {
                    sat_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ceb_id = table.Column<int>(type: "integer", nullable: false),
                    pla_id = table.Column<int>(type: "integer", nullable: false),
                    sat_gravity = table.Column<decimal>(type: "numeric(10,5)", nullable: true),
                    sat_radius = table.Column<decimal>(type: "numeric(7,2)", nullable: true),
                    sat_density = table.Column<decimal>(type: "numeric(6,5)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("satellite_pkey", x => x.sat_id);
                    table.ForeignKey(
                        name: "fk_satellite_celestialbody",
                        column: x => x.ceb_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbody_ceb",
                        principalColumn: "ceb_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_satellite_planet",
                        column: x => x.pla_id,
                        principalSchema: "public",
                        principalTable: "t_e_planet_pla",
                        principalColumn: "pla_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_address_add",
                schema: "public",
                columns: table => new
                {
                    add_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cit_id = table.Column<int>(type: "integer", nullable: false),
                    add_streetnumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    add_streetaddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("address_pkey", x => x.add_id);
                    table.ForeignKey(
                        name: "fk_address_city",
                        column: x => x.cit_id,
                        principalSchema: "public",
                        principalTable: "t_e_city_cit",
                        principalColumn: "cit_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_user_usr",
                schema: "public",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    php_id = table.Column<int>(type: "integer", nullable: true),
                    add_iddelivery = table.Column<int>(type: "integer", nullable: true),
                    add_idinvoicing = table.Column<int>(type: "integer", nullable: true),
                    uro_id = table.Column<int>(type: "integer", nullable: false),
                    usr_lastname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    usr_firstname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    usr_email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    usr_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    usr_username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    usr_password = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: false),
                    usr_avatarurl = table.Column<string>(type: "TEXT", nullable: true),
                    usr_inscriptiondate = table.Column<DateOnly>(type: "date", nullable: false),
                    usr_gender = table.Column<string>(type: "CHAR(1)", nullable: false),
                    usr_ispremium = table.Column<bool>(type: "boolean", nullable: false),
                    usr_multifactorauthentification = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.usr_id);
                    table.ForeignKey(
                        name: "fk_user_deliveryaddress",
                        column: x => x.add_iddelivery,
                        principalSchema: "public",
                        principalTable: "t_e_address_add",
                        principalColumn: "add_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_invoicingaddress",
                        column: x => x.add_idinvoicing,
                        principalSchema: "public",
                        principalTable: "t_e_address_add",
                        principalColumn: "add_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_phoneprefix",
                        column: x => x.php_id,
                        principalSchema: "public",
                        principalTable: "t_e_phoneprefix_php",
                        principalColumn: "php_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_userrole",
                        column: x => x.uro_id,
                        principalSchema: "public",
                        principalTable: "t_e_userrole_uro",
                        principalColumn: "uro_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_article_art",
                schema: "public",
                columns: table => new
                {
                    art_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    art_title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    art_content = table.Column<string>(type: "TEXT", nullable: false),
                    art_ispremium = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("article_pkey", x => x.art_id);
                    table.ForeignKey(
                        name: "fk_article_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_command_cmd",
                schema: "public",
                columns: table => new
                {
                    cmd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    cos_id = table.Column<int>(type: "integer", nullable: false),
                    cmd_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cmd_total = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    cmd_pdfname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cmd_pdfpath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("command_pkey", x => x.cmd_id);
                    table.ForeignKey(
                        name: "fk_command_commandstatus",
                        column: x => x.cos_id,
                        principalSchema: "public",
                        principalTable: "t_e_commandstatus_cos",
                        principalColumn: "cos_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_command_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_creditcard_crc",
                schema: "public",
                columns: table => new
                {
                    crc_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usr_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("creditcard_pkey", x => x.crc_id);
                    table.ForeignKey(
                        name: "fk_creditcard_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_e_discovery_dis",
                schema: "public",
                columns: table => new
                {
                    dis_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ceb_id = table.Column<int>(type: "integer", nullable: false),
                    dst_id = table.Column<int>(type: "integer", nullable: false),
                    als_id = table.Column<int>(type: "integer", nullable: true),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    usr_iddiscapproval = table.Column<int>(type: "integer", nullable: true),
                    usr_idaliasapproval = table.Column<int>(type: "integer", nullable: true),
                    dis_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("discovery_pkey", x => x.dis_id);
                    table.ForeignKey(
                        name: "fk_discovery_aliasapprovaluser",
                        column: x => x.usr_idaliasapproval,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_discovery_aliasstatus",
                        column: x => x.als_id,
                        principalSchema: "public",
                        principalTable: "t_e_aliasstatus_als",
                        principalColumn: "als_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_discovery_approvaluser",
                        column: x => x.usr_iddiscapproval,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_discovery_celestialbody",
                        column: x => x.ceb_id,
                        principalSchema: "public",
                        principalTable: "t_e_celestialbody_ceb",
                        principalColumn: "ceb_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_discovery_discoverystatus",
                        column: x => x.dst_id,
                        principalSchema: "public",
                        principalTable: "t_e_discoverystatus_dst",
                        principalColumn: "dst_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_discovery_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_event_eve",
                schema: "public",
                columns: table => new
                {
                    eve_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    evt_id = table.Column<int>(type: "integer", nullable: false),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    eve_title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    eve_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    eve_startdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    eve_enddate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    eve_location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    eve_link = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("event_pkey", x => x.eve_id);
                    table.ForeignKey(
                        name: "fk_event_eventtype",
                        column: x => x.evt_id,
                        principalSchema: "public",
                        principalTable: "t_e_eventtype_evt",
                        principalColumn: "evt_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_event_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_product_pro",
                schema: "public",
                columns: table => new
                {
                    pro_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prc_id = table.Column<int>(type: "integer", nullable: false),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    pro_label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pro_description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    pro_price = table.Column<decimal>(type: "numeric(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_pkey", x => x.pro_id);
                    table.ForeignKey(
                        name: "fk_product_productcategory",
                        column: x => x.prc_id,
                        principalSchema: "public",
                        principalTable: "t_e_productcategory_prc",
                        principalColumn: "prc_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_j_usernotification_uno",
                schema: "public",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    not_id = table.Column<int>(type: "integer", nullable: false),
                    uno_isread = table.Column<bool>(type: "boolean", nullable: false),
                    uno_receivedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usernotification_pkey", x => new { x.usr_id, x.not_id });
                    table.ForeignKey(
                        name: "fk_usernotification_notification",
                        column: x => x.not_id,
                        principalSchema: "public",
                        principalTable: "t_e_notification_not",
                        principalColumn: "not_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usernotification_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
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
                        principalTable: "t_e_notificationtype_nty",
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

            migrationBuilder.CreateTable(
                name: "t_e_comment_com",
                schema: "public",
                columns: table => new
                {
                    com_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    art_id = table.Column<int>(type: "integer", nullable: false),
                    com_idreply = table.Column<int>(type: "integer", nullable: true),
                    com_text = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    com_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    com_isvisible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("comment_pkey", x => x.com_id);
                    table.ForeignKey(
                        name: "fk_comment_article",
                        column: x => x.art_id,
                        principalSchema: "public",
                        principalTable: "t_e_article_art",
                        principalColumn: "art_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comment_repliesto",
                        column: x => x.com_idreply,
                        principalSchema: "public",
                        principalTable: "t_e_comment_com",
                        principalColumn: "com_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_comment_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_articleinterest_ain",
                schema: "public",
                columns: table => new
                {
                    art_id = table.Column<int>(type: "integer", nullable: false),
                    usr_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("articleinterest_pkey", x => new { x.art_id, x.usr_id });
                    table.ForeignKey(
                        name: "fk_articleinterest_article",
                        column: x => x.art_id,
                        principalSchema: "public",
                        principalTable: "t_e_article_art",
                        principalColumn: "art_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_articleinterest_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_typeofarticle_toa",
                schema: "public",
                columns: table => new
                {
                    aty_id = table.Column<int>(type: "integer", nullable: false),
                    art_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("typeofarticle_pkey", x => new { x.aty_id, x.art_id });
                    table.ForeignKey(
                        name: "fk_typeofarticle_article",
                        column: x => x.art_id,
                        principalSchema: "public",
                        principalTable: "t_e_article_art",
                        principalColumn: "art_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_typeofarticle_articletype",
                        column: x => x.aty_id,
                        principalSchema: "public",
                        principalTable: "t_e_articletype_aty",
                        principalColumn: "aty_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_eventinterest_evi",
                schema: "public",
                columns: table => new
                {
                    eve_id = table.Column<int>(type: "integer", nullable: false),
                    usr_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("eventinterest_pkey", x => new { x.eve_id, x.usr_id });
                    table.ForeignKey(
                        name: "fk_eventinterest_event",
                        column: x => x.eve_id,
                        principalSchema: "public",
                        principalTable: "t_e_event_eve",
                        principalColumn: "eve_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_eventinterest_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_cartitem_cai",
                schema: "public",
                columns: table => new
                {
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    pro_id = table.Column<int>(type: "integer", nullable: false),
                    cai_quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cartitem_pkey", x => new { x.usr_id, x.pro_id });
                    table.ForeignKey(
                        name: "fk_cartitem_product",
                        column: x => x.pro_id,
                        principalSchema: "public",
                        principalTable: "t_e_product_pro",
                        principalColumn: "pro_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cartitem_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_j_orderdetail_ord",
                schema: "public",
                columns: table => new
                {
                    cmd_id = table.Column<int>(type: "integer", nullable: false),
                    pro_id = table.Column<int>(type: "integer", nullable: false),
                    ord_quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orderdetail_pkey", x => new { x.cmd_id, x.pro_id });
                    table.ForeignKey(
                        name: "fk_orderdetail_command",
                        column: x => x.cmd_id,
                        principalSchema: "public",
                        principalTable: "t_e_command_cmd",
                        principalColumn: "cmd_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orderdetail_product",
                        column: x => x.pro_id,
                        principalSchema: "public",
                        principalTable: "t_e_product_pro",
                        principalColumn: "pro_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "t_e_report_rep",
                schema: "public",
                columns: table => new
                {
                    rep_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rst_id = table.Column<int>(type: "integer", nullable: false),
                    rem_id = table.Column<int>(type: "integer", nullable: false),
                    com_id = table.Column<int>(type: "integer", nullable: false),
                    usr_id = table.Column<int>(type: "integer", nullable: false),
                    usr_adminid = table.Column<int>(type: "integer", nullable: true),
                    rep_description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    rep_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("report_pkey", x => x.rep_id);
                    table.ForeignKey(
                        name: "fk_report_adminuser",
                        column: x => x.usr_adminid,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_report_comment",
                        column: x => x.com_id,
                        principalSchema: "public",
                        principalTable: "t_e_comment_com",
                        principalColumn: "com_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_report_reportmotive",
                        column: x => x.rem_id,
                        principalSchema: "public",
                        principalTable: "t_e_reportmotive_rem",
                        principalColumn: "rem_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_report_reportstatus",
                        column: x => x.rst_id,
                        principalSchema: "public",
                        principalTable: "t_e_reportstatus_rst",
                        principalColumn: "rst_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_report_user",
                        column: x => x.usr_id,
                        principalSchema: "public",
                        principalTable: "t_e_user_usr",
                        principalColumn: "usr_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_address_add_cit_id",
                schema: "public",
                table: "t_e_address_add",
                column: "cit_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_article_art_usr_id",
                schema: "public",
                table: "t_e_article_art",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_asteroid_ast_oct_id",
                schema: "public",
                table: "t_e_asteroid_ast",
                column: "oct_id");

            migrationBuilder.CreateIndex(
                name: "uq_asteroid_cebid",
                schema: "public",
                table: "t_e_asteroid_ast",
                column: "ceb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_audio_aud_cbt_id",
                schema: "public",
                table: "t_e_audio_aud",
                column: "cbt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_celestialbody_ceb_cbt_id",
                schema: "public",
                table: "t_e_celestialbody_ceb",
                column: "cbt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_city_cit_cou_id",
                schema: "public",
                table: "t_e_city_cit",
                column: "cou_id");

            migrationBuilder.CreateIndex(
                name: "uq_comet_cebid",
                schema: "public",
                table: "t_e_comet_cmt",
                column: "ceb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_command_cmd_cos_id",
                schema: "public",
                table: "t_e_command_cmd",
                column: "cos_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_command_cmd_usr_id",
                schema: "public",
                table: "t_e_command_cmd",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_comment_com_art_id",
                schema: "public",
                table: "t_e_comment_com",
                column: "art_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_comment_com_com_idreply",
                schema: "public",
                table: "t_e_comment_com",
                column: "com_idreply");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_comment_com_usr_id",
                schema: "public",
                table: "t_e_comment_com",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_country_cou_php_id",
                schema: "public",
                table: "t_e_country_cou",
                column: "php_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_creditcard_crc_usr_id",
                schema: "public",
                table: "t_e_creditcard_crc",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_discovery_dis_als_id",
                schema: "public",
                table: "t_e_discovery_dis",
                column: "als_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_discovery_dis_ceb_id",
                schema: "public",
                table: "t_e_discovery_dis",
                column: "ceb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_discovery_dis_dst_id",
                schema: "public",
                table: "t_e_discovery_dis",
                column: "dst_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_discovery_dis_usr_id",
                schema: "public",
                table: "t_e_discovery_dis",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_discovery_dis_usr_idaliasapproval",
                schema: "public",
                table: "t_e_discovery_dis",
                column: "usr_idaliasapproval");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_discovery_dis_usr_iddiscapproval",
                schema: "public",
                table: "t_e_discovery_dis",
                column: "usr_iddiscapproval");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_event_eve_evt_id",
                schema: "public",
                table: "t_e_event_eve",
                column: "evt_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_event_eve_usr_id",
                schema: "public",
                table: "t_e_event_eve",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_galaxyquasar_gaq_gqc_id",
                schema: "public",
                table: "t_e_galaxyquasar_gaq",
                column: "gqc_id");

            migrationBuilder.CreateIndex(
                name: "uq_galaxyquasar_cebid",
                schema: "public",
                table: "t_e_galaxyquasar_gaq",
                column: "ceb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_notification_not_nty_id",
                schema: "public",
                table: "t_e_notification_not",
                column: "nty_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_planet_pla_dem_id",
                schema: "public",
                table: "t_e_planet_pla",
                column: "dem_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_planet_pla_plt_id",
                schema: "public",
                table: "t_e_planet_pla",
                column: "plt_id");

            migrationBuilder.CreateIndex(
                name: "uq_planet_cebid",
                schema: "public",
                table: "t_e_planet_pla",
                column: "ceb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_product_pro_prc_id",
                schema: "public",
                table: "t_e_product_pro",
                column: "prc_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_product_pro_usr_id",
                schema: "public",
                table: "t_e_product_pro",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_report_rep_com_id",
                schema: "public",
                table: "t_e_report_rep",
                column: "com_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_report_rep_rem_id",
                schema: "public",
                table: "t_e_report_rep",
                column: "rem_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_report_rep_rst_id",
                schema: "public",
                table: "t_e_report_rep",
                column: "rst_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_report_rep_usr_adminid",
                schema: "public",
                table: "t_e_report_rep",
                column: "usr_adminid");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_report_rep_usr_id",
                schema: "public",
                table: "t_e_report_rep",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_satellite_sat_pla_id",
                schema: "public",
                table: "t_e_satellite_sat",
                column: "pla_id");

            migrationBuilder.CreateIndex(
                name: "uq_satellite_cebid",
                schema: "public",
                table: "t_e_satellite_sat",
                column: "ceb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_star_sta_spc_id",
                schema: "public",
                table: "t_e_star_sta",
                column: "spc_id");

            migrationBuilder.CreateIndex(
                name: "uq_star_cebid",
                schema: "public",
                table: "t_e_star_sta",
                column: "ceb_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_add_iddelivery",
                schema: "public",
                table: "t_e_user_usr",
                column: "add_iddelivery");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_add_idinvoicing",
                schema: "public",
                table: "t_e_user_usr",
                column: "add_idinvoicing");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_php_id",
                schema: "public",
                table: "t_e_user_usr",
                column: "php_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_user_usr_uro_id",
                schema: "public",
                table: "t_e_user_usr",
                column: "uro_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_articleinterest_ain_usr_id",
                schema: "public",
                table: "t_j_articleinterest_ain",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_cartitem_cai_pro_id",
                schema: "public",
                table: "t_j_cartitem_cai",
                column: "pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_eventinterest_evi_usr_id",
                schema: "public",
                table: "t_j_eventinterest_evi",
                column: "usr_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_orderdetail_ord_pro_id",
                schema: "public",
                table: "t_j_orderdetail_ord",
                column: "pro_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_typeofarticle_toa_art_id",
                schema: "public",
                table: "t_j_typeofarticle_toa",
                column: "art_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_usernotification_uno_not_id",
                schema: "public",
                table: "t_j_usernotification_uno",
                column: "not_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_j_usernotificationtype_unt_nty_id",
                schema: "public",
                table: "t_j_usernotificationtype_unt",
                column: "nty_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_e_asteroid_ast",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_audio_aud",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_comet_cmt",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_creditcard_crc",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_discovery_dis",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_galaxyquasar_gaq",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_report_rep",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_satellite_sat",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_star_sta",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_j_articleinterest_ain",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_j_cartitem_cai",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_j_eventinterest_evi",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_j_orderdetail_ord",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_j_typeofarticle_toa",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_j_usernotification_uno",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_j_usernotificationtype_unt",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_orbitalclass_oct",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_aliasstatus_als",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_discoverystatus_dst",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_galaxyquasarclass_gqc",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_comment_com",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_reportmotive_rem",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_reportstatus_rst",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_planet_pla",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_spectralclass_spc",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_event_eve",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_command_cmd",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_product_pro",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_articletype_aty",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_notification_not",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_article_art",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_celestialbody_ceb",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_detectionmethod_dem",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_planettype_plt",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_eventtype_evt",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_commandstatus_cos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_productcategory_prc",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_notificationtype_nty",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_user_usr",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_celestialbodytype_cbt",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_address_add",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_userrole_uro",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_city_cit",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_country_cou",
                schema: "public");

            migrationBuilder.DropTable(
                name: "t_e_phoneprefix_php",
                schema: "public");
        }
    }
}
