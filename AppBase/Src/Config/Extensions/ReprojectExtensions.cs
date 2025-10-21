using Dapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace AppBase.Config.Extensions;

public static class ReprojectExtensions
{
    public static async Task<Point?> ReprojectPointAsync(this DbContext dbContext, Point point, int sourceSrid,
        int targetSrid)
    {
        var result = await ReprojectGeometryAsync(dbContext, point, sourceSrid, targetSrid);
        return result as Point;
    }

    public static async Task<LineString?> ReprojectLineStringAsync(this DbContext dbContext, LineString lineString,
        int sourceSrid, int targetSrid)
    {
        var result = await ReprojectGeometryAsync(dbContext, lineString, sourceSrid, targetSrid);
        return result as LineString;
    }

    public static async Task<Polygon?> ReprojectPolygonAsync(this DbContext dbContext, Polygon polygon, int sourceSrid,
        int targetSrid)
    {
        var result = await ReprojectGeometryAsync(dbContext, polygon, sourceSrid, targetSrid);
        return result as Polygon;
    }

    public static async Task<MultiPoint?> ReprojectMultiPointAsync(this DbContext dbContext, MultiPoint multiPoint,
        int sourceSrid, int targetSrid)
    {
        var result = await ReprojectGeometryAsync(dbContext, multiPoint, sourceSrid, targetSrid);
        return result as MultiPoint;
    }

    private static async Task<Geometry?> ReprojectGeometryAsync(this DbContext dbContext, Geometry geometry,
        int sourceSrid, int targetSrid)
    {
        var connection = dbContext.Database.GetDbConnection();

        var wkt = await connection.ExecuteScalarAsync<string>(
            "SELECT ST_AsText(ST_Transform(ST_GeomFromText(@wkt, @sourceSrid), @targetSrid))",
            new
            {
                wkt = geometry.AsText(),
                sourceSrid,
                targetSrid
            });

        if (!string.IsNullOrEmpty(wkt))
        {
            var reader = new WKTReader();
            var reprojected = reader.Read(wkt);
            reprojected.SRID = targetSrid;
            return reprojected;
        }

        return null;
    }
}