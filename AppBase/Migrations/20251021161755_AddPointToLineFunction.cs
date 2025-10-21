using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBase.Migrations
{
   public partial class AddPointToLineFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION add_point_to_line_nearest_end(
                line_wkt TEXT,
                point_wkt TEXT
            )
            RETURNS TEXT
            LANGUAGE plpgsql
            AS $$
            DECLARE
                line_geometry GEOMETRY;
                point_geometry GEOMETRY;
                start_point GEOMETRY;
                end_point GEOMETRY;
                distance_to_start FLOAT;
                distance_to_end FLOAT;
                result_geometry GEOMETRY;
            BEGIN
                -- Convert WKT to geometries
                line_geometry := ST_GeomFromText(line_wkt, 4326);
                point_geometry := ST_GeomFromText(point_wkt, 4326);

                -- Get start and end points of the line
                start_point := ST_StartPoint(line_geometry);
                end_point := ST_EndPoint(line_geometry);

                -- Calculate distances
                distance_to_start := ST_Distance(point_geometry, start_point);
                distance_to_end := ST_Distance(point_geometry, end_point);

                -- Add point to start or end based on proximity
                IF distance_to_start <= distance_to_end THEN
                    -- Add to start (position 0)
                    result_geometry := ST_AddPoint(line_geometry, point_geometry, 0);
                ELSE
                    -- Add to end (position -1)
                    result_geometry := ST_AddPoint(line_geometry, point_geometry);
                END IF;

                -- Return the new line as WKT
                RETURN ST_AsText(result_geometry);
            END;
            $$;
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION add_point_to_line_nearest_end;");
        }
    }
}
