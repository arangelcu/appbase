using AppBase.Config.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace AppBase.Utils.Geometry;

public class LineStringUtils
{
    public static LineString AddPointToStart(LineString line, Point point)
    {
        var coordinates = line.Coordinates.ToList();
        coordinates.Insert(0, point.Coordinate);
        return new LineString(coordinates.ToArray());
    }

    public static LineString AddPointToEnd(LineString line, Point point)
    {
        var coordinates = line.Coordinates.ToList();
        coordinates.Add(point.Coordinate);
        return new LineString(coordinates.ToArray());
    }

    public static LineString AddPointToClosestLinestringEndpoint(LineString line, Point point)
    {
        if (line.Contains(point))
        {
            Console.WriteLine("Point is already inside the LineString. No need to add.");
            return line;
        }

        var distanceToStart = point.Distance(line.StartPoint);
        var distanceToEnd = point.Distance(line.EndPoint);

        Console.WriteLine($"Distance to start: {distanceToStart}");
        Console.WriteLine($"Distance to end: {distanceToEnd}");

        if (distanceToStart <= distanceToEnd)
        {
            Console.WriteLine("Adding to START of the line");
            return AddPointToStart(line, point);
        }

        Console.WriteLine("Adding to END of the line");
        return AddPointToEnd(line, point);
    }

    public static async Task<LineString> AddPointToClosestLinestringEndpointPostGis(ApiDbContext dbcontext,
        LineString line, Point point)
    {
        if (line.Contains(point))
        {
            Console.WriteLine("Point is already inside the LineString. No need to add.");
            return line;
        }

        var lineWkt = line.AsText();
        var pointWkt = point.AsText();

        var result = await dbcontext.Database
            .SqlQueryRaw<SqlResult>($@"SELECT add_point_to_line_nearest_end('{lineWkt}', '{pointWkt}') as ""Value""")
            .FirstOrDefaultAsync();

        if (result == null || string.IsNullOrEmpty(result.Value))
            throw new InvalidOperationException("Error_PostGIS_Fn_Exec");

        var reader = new WKTReader();
        return (LineString)reader.Read(result.Value);
    }

    public static LineString RemoveClosestLinestringPointFromReferencePoint(LineString line, Point point)
    {
        Console.WriteLine("Find the index of the closest point to the reference point.");
        int closestIndex = -1;
        double minDistance = double.MaxValue;

        for (int i = 0; i < line.Coordinates.Length; i++)
        {
            double distance = point.Distance(new Point(line.Coordinates[i]));
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        Console.WriteLine("Create new coordinates array without the closest point.");
        var newCoordinates = new Coordinate[line.Coordinates.Length - 1];
        int newIndex = 0;

        for (int i = 0; i < line.Coordinates.Length; i++)
        {
            if (i != closestIndex)
            {
                newCoordinates[newIndex] = line.Coordinates[i];
                newIndex++;
            }
        }

        return new LineString(newCoordinates.ToArray());
    }

    public static LineString ApplyBezierSmoothingToLinestring(LineString line, double intensity)
    {
        // Number of intermediate points to generate between each original point
        int segments = (int)(10 * intensity) + 2; // 2 to 12 segments based on intensity

        var smoothedPoints = new List<Coordinate>();

        // Always include the first point
        smoothedPoints.Add(line.Coordinates[0]);

        Console.WriteLine("Generate smoothed points for each segment.");
        for (int i = 0; i < line.Coordinates.Length - 1; i++)
        {
            Coordinate p0 = (i == 0) ? line.Coordinates[0] : line.Coordinates[i - 1];
            Coordinate p1 = line.Coordinates[i];
            Coordinate p2 = line.Coordinates[i + 1];
            Coordinate p3 = (i == line.Coordinates.Length - 2)
                ? line.Coordinates[line.Coordinates.Length - 1]
                : line.Coordinates[i + 2];
          
            Console.WriteLine("Generate intermediate points using cubic Bezier.");
            for (int j = 1; j < segments; j++)
            {
                double t = (double)j / segments;
                Coordinate smoothedPoint = CalculateCubicBezierPoint(p0, p1, p2, p3, t, intensity);
                smoothedPoints.Add(smoothedPoint);
            }

            Console.WriteLine(" Add the original control point (with reduced influence based on intensity).");
            if (i < line.Coordinates.Length - 1)
            {
                smoothedPoints.Add(p2);
            }
        }

        return new LineString(smoothedPoints.ToArray());
    }

    private static Coordinate CalculateCubicBezierPoint(Coordinate p0, Coordinate p1, Coordinate p2, Coordinate p3,
        double t,
        double intensity)
    {
        // Adjust control points based on intensity
        double tension = 0.5 * intensity;

        // Calculate intermediate control points
        Coordinate cp1 = new Coordinate(
            p1.X + tension * (p2.X - p0.X),
            p1.Y + tension * (p2.Y - p0.Y)
        );

        Coordinate cp2 = new Coordinate(
            p2.X - tension * (p3.X - p1.X),
            p2.Y - tension * (p3.Y - p1.Y)
        );

        // Cubic Bezier formula: B(t) = (1-t)³P0 + 3(1-t)²tP1 + 3(1-t)t²P2 + t³P3
        double u = 1 - t;
        double u2 = u * u;
        double t2 = t * t;
        double u3 = u2 * u;
        double t3 = t2 * t;

        double x = u3 * p1.X + 3 * u2 * t * cp1.X + 3 * u * t2 * cp2.X + t3 * p2.X;
        double y = u3 * p1.Y + 3 * u2 * t * cp1.Y + 3 * u * t2 * cp2.Y + t3 * p2.Y;

        return new Coordinate(x, y);
    }
}