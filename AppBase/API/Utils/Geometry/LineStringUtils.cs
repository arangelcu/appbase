using AppBase.API.Config.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace AppBase.API.Utils.Geometry;

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
}