using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBase.Migrations
{
   public partial class AddPointToPolygonClosestEdgeFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION add_point_to_polygon_closest_edge(
                polygon_wkt TEXT,
                point_wkt TEXT
            )
            RETURNS TEXT
            LANGUAGE plpgsql
            AS $$
            DECLARE
                polygon_geometry GEOMETRY;
                point_geometry GEOMETRY;
                exterior_ring GEOMETRY;
                exterior_coordinates GEOMETRY[];
                closest_edge_index INTEGER;
                min_distance FLOAT;
                current_distance FLOAT;
                i INTEGER;
                new_exterior_coordinates GEOMETRY[];
                new_exterior_ring GEOMETRY;
                new_polygon GEOMETRY;
            BEGIN
                -- Convert WKT to geometries
                polygon_geometry := ST_GeomFromText(polygon_wkt, 4326);
                point_geometry := ST_GeomFromText(point_wkt, 4326);

                -- Validate geometry types
                IF ST_GeometryType(polygon_geometry) != 'ST_Polygon' THEN
                    RAISE EXCEPTION 'First parameter must be a Polygon';
                END IF;
                
                IF ST_GeometryType(point_geometry) != 'ST_Point' THEN
                    RAISE EXCEPTION 'Second parameter must be a Point';
                END IF;

                -- Get exterior ring
                exterior_ring := ST_ExteriorRing(polygon_geometry);
                
                -- Validate minimum points in exterior ring
                IF ST_NPoints(exterior_ring) < 4 THEN -- 3 points + closing point
                    RAISE EXCEPTION 'Polygon must have at least 3 distinct points';
                END IF;

                -- Extract coordinates from exterior ring (excluding the closing point)
                FOR i IN 1..(ST_NPoints(exterior_ring) - 1) LOOP
                    exterior_coordinates := array_append(
                        exterior_coordinates, 
                        ST_PointN(exterior_ring, i)
                    );
                END LOOP;

                -- Find closest edge to the point
                min_distance := 'Infinity'::FLOAT;
                closest_edge_index := -1;
                
                FOR i IN 1..array_length(exterior_coordinates, 1) LOOP
                    -- Calculate distance from point to edge (line segment between current and next point)
                    IF i = array_length(exterior_coordinates, 1) THEN
                        -- Last edge: from last point to first point
                        current_distance := ST_Distance(
                            point_geometry,
                            ST_MakeLine(
                                exterior_coordinates[i],
                                exterior_coordinates[1]
                            )
                        );
                    ELSE
                        -- Normal edge: from current point to next point
                        current_distance := ST_Distance(
                            point_geometry,
                            ST_MakeLine(
                                exterior_coordinates[i],
                                exterior_coordinates[i + 1]
                            )
                        );
                    END IF;
                    
                    IF current_distance < min_distance THEN
                        min_distance := current_distance;
                        closest_edge_index := i;
                    END IF;
                END LOOP;

                -- Build new coordinates array with inserted point
                FOR i IN 1..array_length(exterior_coordinates, 1) LOOP
                    -- Add current coordinate
                    new_exterior_coordinates := array_append(
                        new_exterior_coordinates, 
                        exterior_coordinates[i]
                    );
                    
                    -- Insert new point after the closest edge start point
                    IF i = closest_edge_index THEN
                        IF i = array_length(exterior_coordinates, 1) THEN
                            -- For last edge, insert between last and first point
                            new_exterior_coordinates := array_append(
                                new_exterior_coordinates, 
                                point_geometry
                            );
                        ELSE
                            -- For normal edges, insert between current and next point
                            new_exterior_coordinates := array_append(
                                new_exterior_coordinates, 
                                point_geometry
                            );
                        END IF;
                    END IF;
                END LOOP;

                -- Close the ring by adding the first point at the end
                new_exterior_coordinates := array_append(
                    new_exterior_coordinates, 
                    new_exterior_coordinates[1]
                );

                -- Create new exterior ring
                new_exterior_ring := ST_MakeLine(new_exterior_coordinates);
                
                -- Create new polygon (handling interior rings if any)
                IF ST_NumInteriorRings(polygon_geometry) > 0 THEN
                    -- Preserve interior rings
                    new_polygon := ST_MakePolygon(
                        new_exterior_ring,
                        (SELECT array_agg(ST_InteriorRingN(polygon_geometry, n))
                         FROM generate_series(1, ST_NumInteriorRings(polygon_geometry)) AS n)
                    );
                ELSE
                    -- No interior rings
                    new_polygon := ST_MakePolygon(new_exterior_ring);
                END IF;

                -- Validate the resulting polygon
                IF NOT ST_IsValid(new_polygon) THEN
                    RAISE EXCEPTION 'Resulting polygon geometry is invalid';
                END IF;

                RETURN ST_AsText(new_polygon);
            END;
            $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION add_point_to_polygon_closest_edge;");
        }
    }
}
