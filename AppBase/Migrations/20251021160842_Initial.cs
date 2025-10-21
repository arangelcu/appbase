using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AppBase.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "landmarks",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    geometry = table.Column<Point>(type: "geometry(Point, 4326)", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_landmarks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "squares",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    geometry = table.Column<Polygon>(type: "geometry(Polygon, 4326)", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_squares", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "streets",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    geometry = table.Column<LineString>(type: "geometry(LineString, 4326)", nullable: false),
                    update_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_streets", x => x.id);
                });
            
            // INITIAL DATA FOR STREETS
            migrationBuilder.InsertData(
                schema: "public",
                table: "streets",
                columns: new[] { "name", "description", "capacity", "geometry", "update_at" },
                values: new object[,]
                {
                    {
                        "Kaiserstraße", "Main commercial and pedestrian street in the city center", 800,
                        "LINESTRING(8.40379 49.00939, 8.40410 49.00950, 8.40445 49.00962, 8.40486 49.00974)", DateTime.UtcNow
                    },
                    {
                        "Kaiserstraße", "Main commercial and pedestrian street in the city center", 750,
                        "LINESTRING(8.40486 49.00974, 8.40525 49.00985, 8.40565 49.00996, 8.40612 49.01012)", DateTime.UtcNow
                    },
                    {
                        "Karl-Friedrich-Straße", "Historic street near the palace", 300,
                        "LINESTRING(8.40345 49.00871, 8.40365 49.00882, 8.40385 49.00893, 8.40405 49.00904, 8.40420 49.00915)", DateTime.UtcNow
                    },
                    {
                        "Waldstraße", "Street with traditional architecture and shops", 350,
                        "LINESTRING(8.40628 49.01021, 8.40655 49.01032, 8.40682 49.01043, 8.40710 49.01054, 8.40750 49.01065)", DateTime.UtcNow
                    },
                    {
                        "Kriegsstraße", "Major traffic artery with high vehicle flow", 1200,
                        "LINESTRING(8.38667 49.01082, 8.38750 49.01085, 8.38830 49.01088, 8.38920 49.01095, 8.39011 49.01103)", DateTime.UtcNow
                    },
                    {
                        "Hans-Thoma-Straße", "Residential street with historic buildings", 250,
                        "LINESTRING(8.40189 49.00894, 8.40215 49.00902, 8.40240 49.00910, 8.40265 49.00921, 8.40295 49.00933)", DateTime.UtcNow
                    },
                    {
                        "Ettlinger Straße", "Main axis towards the southern part of the city", 600,
                        "LINESTRING(8.40340 49.00645, 8.40365 49.00658, 8.40390 49.00668, 8.40415 49.00678, 8.40438 49.00689)", DateTime.UtcNow
                    },
                    {
                        "Rüppurrer Straße", "Connects city center with Rüppurr district", 550,
                        "LINESTRING(8.40438 49.00689, 8.40465 49.00698, 8.40490 49.00708, 8.40515 49.00718, 8.40542 49.00730)", DateTime.UtcNow
                    },
                    {
                        "Sophienstraße", "Quiet street with residential atmosphere", 200,
                        "LINESTRING(8.40752 49.01066, 8.40775 49.01075, 8.40798 49.01082, 8.40825 49.01092, 8.40861 49.01105)", DateTime.UtcNow
                    },
                    {
                        "Zähringerstraße", "Street with 19th century architecture", 180,
                        "LINESTRING(8.40861 49.01105, 8.40885 49.01115, 8.40912 49.01125, 8.40938 49.01135, 8.40977 49.01147)", DateTime.UtcNow
                    },
                    {
                        "Douglasstraße", "Commercial street near main train station", 400,
                        "LINESTRING(8.40079 49.00970, 8.40105 49.00978, 8.40132 49.00986, 8.40158 49.00996, 8.40189 49.01010)", DateTime.UtcNow
                    },
                    {
                        "Bahnhofsplatz", "Station square with multiple traffic lanes", 500,
                        "LINESTRING(8.40050 49.00920, 8.40065 49.00928, 8.40082 49.00935, 8.40100 49.00942, 8.40120 49.00950)", DateTime.UtcNow
                    },
                    {
                        "Karlstraße", "Shopping street with moderate traffic", 350,
                        "LINESTRING(8.40542 49.01012, 8.40565 49.01020, 8.40588 49.01028, 8.40608 49.01036, 8.40628 49.01045)", DateTime.UtcNow
                    },
                    {
                        "Lammstraße", "Street with restaurants and cafes", 280,
                        "LINESTRING(8.40420 49.00820, 8.40440 49.00828, 8.40460 49.00835, 8.40480 49.00842, 8.40500 49.00850)", DateTime.UtcNow
                    },
                    {
                        "Mendelssohnplatz", "Circular street around Mendelssohn square", 320,
                        "LINESTRING(8.40700 49.00900, 8.40720 49.00908, 8.40740 49.00915, 8.40760 49.00922, 8.40780 49.00930)", DateTime.UtcNow
                    }
                });

            // INITIAL DATA FOR SQUARES
            migrationBuilder.InsertData(
                schema: "public",
                table: "squares",
                columns: new[] { "name", "description", "capacity", "geometry", "update_at" },
                values: new object[,]
                {
                    {
                        "Marktplatz", "Main market square with town hall and church", 8000,
                        "POLYGON((8.40393 49.00949, 8.40415 49.00948, 8.40435 49.00947, 8.40455 49.00946, 8.40476 49.00948, 8.40475 49.00935, 8.40474 49.00922, 8.40473 49.00910, 8.40465 49.00911, 8.40445 49.00912, 8.40425 49.00913, 8.40405 49.00914, 8.40392 49.00911, 8.40393 49.00925, 8.40393 49.00938, 8.40393 49.00949))", DateTime.UtcNow
                    },
                    {
                        "Schlossplatz", "Palace square with gardens and museums", 15000,
                        "POLYGON((8.40429 49.01339, 8.40445 49.01340, 8.40465 49.01341, 8.40485 49.01342, 8.40505 49.01343, 8.40525 49.01340, 8.40524 49.01325, 8.40523 49.01310, 8.40522 49.01295, 8.40521 49.01290, 8.40505 49.01291, 8.40485 49.01292, 8.40465 49.01293, 8.40445 49.01294, 8.40429 49.01290, 8.40430 49.01305, 8.40431 49.01320, 8.40432 49.01335, 8.40429 49.01339))", DateTime.UtcNow
                    },
                    {
                        "Friedrichsplatz", "Square with fountains and cultural events", 5000,
                        "POLYGON((8.40079 49.00859, 8.40100 49.00858, 8.40120 49.00857, 8.40140 49.00856, 8.40165 49.00859, 8.40164 49.00845, 8.40163 49.00832, 8.40162 49.00820, 8.40140 49.00821, 8.40120 49.00822, 8.40100 49.00823, 8.40079 49.00820, 8.40080 49.00830, 8.40081 49.00840, 8.40082 49.00850, 8.40079 49.00859))", DateTime.UtcNow
                    },
                    {
                        "Ettlinger Tor Platz", "Modern square with underground shopping center", 6000,
                        "POLYGON((8.40340 49.00645, 8.40360 49.00644, 8.40380 49.00643, 8.40400 49.00642, 8.40420 49.00641, 8.40438 49.00645, 8.40437 49.00630, 8.40436 49.00615, 8.40435 49.00600, 8.40434 49.00590, 8.40420 49.00591, 8.40400 49.00592, 8.40380 49.00593, 8.40360 49.00594, 8.40340 49.00590, 8.40341 49.00605, 8.40342 49.00620, 8.40343 49.00635, 8.40340 49.00645))", DateTime.UtcNow
                    },
                    {
                        "Rondellplatz", "Historic circular square in the city center", 3000,
                        "POLYGON((8.40300 49.01100, 8.40312 49.01102, 8.40325 49.01103, 8.40338 49.01104, 8.40350 49.01100, 8.40348 49.01085, 8.40346 49.01070, 8.40344 49.01055, 8.40342 49.01050, 8.40330 49.01051, 8.40315 49.01052, 8.40305 49.01053, 8.40300 49.01050, 8.40301 49.01065, 8.40302 49.01080, 8.40303 49.01095, 8.40300 49.01100))", DateTime.UtcNow
                    },
                    {
                        "Europäischer Platz", "Square near main train station", 10000,
                        "POLYGON((8.40000 49.01000, 8.40040 49.00998, 8.40080 49.00996, 8.40120 49.00994, 8.40150 49.01000, 8.40148 49.00980, 8.40146 49.00960, 8.40144 49.00940, 8.40142 49.00900, 8.40120 49.00902, 8.40080 49.00904, 8.40040 49.00906, 8.40000 49.00900, 8.40002 49.00920, 8.40004 49.00940, 8.40006 49.00960, 8.40008 49.00980, 8.40000 49.01000))", DateTime.UtcNow
                    },
                    {
                        "Ludwigsplatz", "Square with weekly market and surrounding shops", 4000,
                        "POLYGON((8.40600 49.00900, 8.40625 49.00898, 8.40650 49.00896, 8.40675 49.00894, 8.40700 49.00900, 8.40698 49.00880, 8.40696 49.00860, 8.40694 49.00850, 8.40675 49.00852, 8.40650 49.00854, 8.40625 49.00856, 8.40600 49.00850, 8.40602 49.00865, 8.40604 49.00880, 8.40606 49.00895, 8.40600 49.00900))", DateTime.UtcNow
                    },
                    {
                        "Mühlburger Tor Platz", "Square at historical city gate location", 2500,
                        "POLYGON((8.39200 49.00800, 8.39225 49.00798, 8.39250 49.00796, 8.39275 49.00794, 8.39300 49.00800, 8.39298 49.00780, 8.39296 49.00760, 8.39294 49.00750, 8.39275 49.00752, 8.39250 49.00754, 8.39225 49.00756, 8.39200 49.00750, 8.39202 49.00765, 8.39204 49.00780, 8.39206 49.00795, 8.39200 49.00800))", DateTime.UtcNow
                    },
                    {
                        "Durlacher Tor Platz", "Eastern entrance square to city center", 3500,
                        "POLYGON((8.40800 49.00800, 8.40825 49.00798, 8.40850 49.00796, 8.40875 49.00794, 8.40900 49.00800, 8.40898 49.00780, 8.40896 49.00760, 8.40894 49.00750, 8.40875 49.00752, 8.40850 49.00754, 8.40825 49.00756, 8.40800 49.00750, 8.40802 49.00765, 8.40804 49.00780, 8.40806 49.00795, 8.40800 49.00800))", DateTime.UtcNow
                    },
                    {
                        "Stephanplatz", "Residential square in Weststadt district", 2000,
                        "POLYGON((8.39500 49.00700, 8.39525 49.00698, 8.39550 49.00696, 8.39575 49.00694, 8.39600 49.00700, 8.39598 49.00680, 8.39596 49.00660, 8.39594 49.00650, 8.39575 49.00652, 8.39550 49.00654, 8.39525 49.00656, 8.39500 49.00650, 8.39502 49.00665, 8.39504 49.00680, 8.39506 49.00695, 8.39500 49.00700))", DateTime.UtcNow
                    },
                    {
                        "Gutenbergplatz", "Square with playground and green areas", 1800,
                        "POLYGON((8.39700 49.01200, 8.39725 49.01198, 8.39750 49.01196, 8.39775 49.01194, 8.39800 49.01200, 8.39798 49.01180, 8.39796 49.01160, 8.39794 49.01150, 8.39775 49.01152, 8.39750 49.01154, 8.39725 49.01156, 8.39700 49.01150, 8.39702 49.01165, 8.39704 49.01180, 8.39706 49.01195, 8.39700 49.01200))", DateTime.UtcNow
                    },
                    {
                        "Mendelssohnplatz", "Circular square in Südstadt district", 2200,
                        "POLYGON((8.40650 49.00600, 8.40665 49.00598, 8.40685 49.00596, 8.40705 49.00594, 8.40725 49.00592, 8.40750 49.00600, 8.40748 49.00585, 8.40746 49.00570, 8.40744 49.00555, 8.40742 49.00550, 8.40725 49.00552, 8.40705 49.00554, 8.40685 49.00556, 8.40665 49.00558, 8.40650 49.00550, 8.40652 49.00565, 8.40654 49.00580, 8.40656 49.00595, 8.40650 49.00600))", DateTime.UtcNow
                    },
                    {
                        "Rüppurrer Tor Platz", "Southern entrance square to city center", 2800,
                        "POLYGON((8.40400 49.00500, 8.40425 49.00498, 8.40450 49.00496, 8.40475 49.00494, 8.40500 49.00500, 8.40498 49.00480, 8.40496 49.00460, 8.40494 49.00450, 8.40475 49.00452, 8.40450 49.00454, 8.40425 49.00456, 8.40400 49.00450, 8.40402 49.00465, 8.40404 49.00480, 8.40406 49.00495, 8.40400 49.00500))", DateTime.UtcNow
                    },
                    {
                        "Werderplatz", "Square near Bundesgerichtshof (Federal Court)", 3200,
                        "POLYGON((8.39000 49.00600, 8.39025 49.00598, 8.39050 49.00596, 8.39075 49.00594, 8.39100 49.00600, 8.39098 49.00580, 8.39096 49.00560, 8.39094 49.00550, 8.39075 49.00552, 8.39050 49.00554, 8.39025 49.00556, 8.39000 49.00550, 8.39002 49.00565, 8.39004 49.00580, 8.39006 49.00595, 8.39000 49.00600))", DateTime.UtcNow
                    },
                    {
                        "Kronenplatz", "Major intersection and public transport hub", 7000,
                        "POLYGON((8.40200 49.00700, 8.40230 49.00698, 8.40260 49.00696, 8.40290 49.00694, 8.40320 49.00692, 8.40350 49.00700, 8.40348 49.00680, 8.40346 49.00660, 8.40344 49.00640, 8.40342 49.00600, 8.40320 49.00602, 8.40290 49.00604, 8.40260 49.00606, 8.40230 49.00608, 8.40200 49.00600, 8.40202 49.00620, 8.40204 49.00640, 8.40206 49.00660, 8.40208 49.00680, 8.40210 49.00695, 8.40200 49.00700))", DateTime.UtcNow
                    }
                });

            // INITIAL DATA FOR LANDMARKS
            migrationBuilder.InsertData(
                schema: "public",
                table: "landmarks",
                columns: new[] { "name", "description", "geometry", "update_at" },
                values: new object[,]
                {
                    {
                        "Karlsruhe Palace", "Historic palace built in 1715, now houses the Baden State Museum",
                        "POINT(8.40428 49.01389)", DateTime.UtcNow
                    },
                    {
                        "Karlsruhe Pyramid", "Landmark pyramid in the market square marking the tomb of city founder",
                        "POINT(8.40435 49.00935)", DateTime.UtcNow
                    },
                    {
                        "Karlsruhe Hauptbahnhof", "Main railway station with historic building and modern shopping center",
                        "POINT(8.40083 49.00972)", DateTime.UtcNow
                    },
                    {
                        "Baden State Theater", "Premiere theater for opera, ballet, and drama performances",
                        "POINT(8.40389 49.00889)", DateTime.UtcNow
                    },
                    {
                        "ZKM Center for Art and Media", "World-renowned museum for interactive and media arts",
                        "POINT(8.38583 49.00972)", DateTime.UtcNow
                    },
                    {
                        "St. Stephen's Church", "Beautiful Catholic church with distinctive green copper dome",
                        "POINT(8.39528 49.00750)", DateTime.UtcNow
                    },
                    {
                        "Federal Constitutional Court", "Highest court in Germany, located in Karlsruhe",
                        "POINT(8.39000 49.01278)", DateTime.UtcNow
                    },
                    {
                        "Karlsruhe Zoo", "Popular city zoo with over 800 animals and botanical gardens",
                        "POINT(8.42500 49.00833)", DateTime.UtcNow
                    },
                    {
                        "State Museum of Natural History", "Natural history museum with extensive geological and biological collections",
                        "POINT(8.41028 49.01222)", DateTime.UtcNow
                    },
                    {
                        "Europahalle", "Multi-purpose arena for concerts, sports events, and exhibitions",
                        "POINT(8.42000 49.01500)", DateTime.UtcNow
                    },
                    {
                        "Botanical Garden", "University botanical garden with diverse plant collections",
                        "POINT(8.41778 49.01250)", DateTime.UtcNow
                    },
                    {
                        "Turmberg Tower", "Observation tower on Turmberg hill with panoramic city views",
                        "POINT(8.46833 48.99583)", DateTime.UtcNow
                    },
                    {
                        "Gottesaue Palace", "Renaissance palace now used as University of Music building",
                        "POINT(8.43056 49.01639)", DateTime.UtcNow
                    },
                    {
                        "Market Hall", "Historic market hall with food stalls and regional specialties",
                        "POINT(8.40194 49.00806)", DateTime.UtcNow
                    },
                    {
                        "Art Gallery Karlsruhe", "Important art museum with collections from medieval to contemporary art",
                        "POINT(8.40278 49.01389)", DateTime.UtcNow
                    }
                });

            migrationBuilder.CreateIndex(
                name: "Idx_LandMark_Name_Unique",
                schema: "public",
                table: "landmarks",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Idx_Polygon_Name_Unique",
                schema: "public",
                table: "squares",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Idx_Street_Name_Unique",
                schema: "public",
                table: "streets",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "landmarks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "squares",
                schema: "public");

            migrationBuilder.DropTable(
                name: "streets",
                schema: "public");
        }
    }
}
