using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBase.Migrations
{
    public partial class RemoveClosestPointFromLineFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION remove_closest_point_from_line(
                line_wkt TEXT,
                point_wkt TEXT
            )
            RETURNS TEXT
            LANGUAGE plpgsql
            AS $$
            DECLARE
                line_geometry GEOMETRY;
                point_geometry GEOMETRY;
                closest_point GEOMETRY;
                closest_index INTEGER;
                min_distance FLOAT;
                current_distance FLOAT;
                points_array GEOMETRY[];
                new_points_array GEOMETRY[];
                i INTEGER;
            BEGIN
                -- Convert WKT to geometries
                line_geometry := ST_GeomFromText(line_wkt, 4326);
                point_geometry := ST_GeomFromText(point_wkt, 4326);
                
                -- Validate that the line has at least 3 points
                IF ST_NPoints(line_geometry) < 3 THEN
                    RAISE EXCEPTION 'Line must have at least 3 points to remove one.';
                END IF;

                -- Initialize variables
                min_distance := 'Infinity'::FLOAT;
                closest_index := -1;
                
                -- Find the closest point to the reference point
                FOR i IN 1..ST_NPoints(line_geometry) LOOP
                    current_distance := ST_Distance(
                        point_geometry, 
                        ST_PointN(line_geometry, i)
                    );
                    
                    IF current_distance < min_distance THEN
                        min_distance := current_distance;
                        closest_index := i;
                    END IF;
                END LOOP;

                -- Build new points array excluding the closest point
                FOR i IN 1..ST_NPoints(line_geometry) LOOP
                    IF i != closest_index THEN
                        new_points_array := array_append(
                            new_points_array, 
                            ST_PointN(line_geometry, i)
                        );
                    END IF;
                END LOOP;

                -- Create new line from the remaining points
                RETURN ST_AsText(ST_MakeLine(new_points_array));
            END;
            $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION remove_closest_point_from_line;");
        }
    }
}
