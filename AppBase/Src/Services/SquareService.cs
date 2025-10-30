using System.Data;
using AppBase.Config.Data;
using AppBase.Model.Dto;
using AppBase.Model.Entity;
using AppBase.Model.Repositories;
using AppBase.Utils;
using AppBase.Utils.Geometry;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace AppBase.Services;

public class SquareService : ISquareService
{
    private readonly ApiDbContext _dbContext;
    private readonly ISquareRepository _squareRepository;


    public SquareService(ApiDbContext dbContext, ISquareRepository squareRepository)
    {
        _dbContext = dbContext;
        _squareRepository = squareRepository;
    }

    public async Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable)
    {
        var obj = await _squareRepository.GetAll(name, description, pageable);

        if (geojson is true)
        {
            var featureCollection = new FeatureCollection();
            foreach (var item in obj.Data)
                featureCollection.Add(new Feature
                {
                    Geometry = item.Geometry,
                    Attributes = new AttributesTable
                    {
                        { "id", item.Id },
                        { "name", item.Name },
                        { "description", item.Description },
                        { "capacity", item.Capacity }
                    }
                });

            var writer = new GeoJsonWriter();
            var geoJson = writer.Write(featureCollection);

            return new ContentResult
            {
                Content = geoJson,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        return new OkObjectResult(obj);
    }

    public async Task<ActionResult> GetById(int id)
    {
        var obj = await _dbContext.Squares
            .Where(r => r.Id == id)
            .Select(r => new SquareResDto
            {
                Name = r.Name,
                Description = r.Description,
                Geometry = (Polygon)r.Geometry,
                Capacity = r.Capacity
            })
            .FirstOrDefaultAsync();
        return obj == null ? new NotFoundObjectResult(Message.Warning_NotFound) : new OkObjectResult(obj);
    }

    public async Task<ActionResult> Add(SquareReqDto dto)
    {
        var obj = new Square
        {
            Name = dto.Name,
            Description = dto.Description,
            UpdateAt = DateTime.UtcNow,
            Geometry = dto.Geometry,
            Capacity = dto.Capacity
        };
        _dbContext.Squares.Add(obj);
        await _dbContext.SaveChangesAsync();

        return new OkObjectResult(new SquareResDto
        {
            Id = obj.Id,
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<ActionResult> Update(int id, SquareReqDto dto)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Squares.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        obj.Name = dto.Name;
        obj.Description = dto.Description;
        obj.UpdateAt = DateTime.UtcNow;
        obj.Geometry = dto.Geometry;
        obj.Capacity = obj.Capacity;

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new OkObjectResult(new SquareResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<ActionResult> Delete(int id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Squares.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        _dbContext.Squares.Remove(obj);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return new OkObjectResult(Message.Resource_Deleted);
    }

    public async Task<IActionResult> AddPointToSquare(int id, GeometryUpdDto dto)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Squares.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        if (obj.Geometry == null)
            return new BadRequestObjectResult("Invalid geometry. Expected Polygon.");

        if (dto.Postgis is true)
        {
            obj.Geometry = await PolygonUtils.AddPointToPolygonClosestEdgePostGis(_dbContext, obj.Geometry, dto.Point);
        }
        else
        {
            obj.Geometry = PolygonUtils.AddPointToPolygonClosestEdge(obj.Geometry, dto.Point);
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return new OkObjectResult(new SquareResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<IActionResult> CheckSquareContainPoint(int id, GeometryUpdDto dto)
    {
        var obj = await _dbContext.Squares.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        if (obj.Geometry == null)
            return new BadRequestObjectResult("Invalid geometry. Expected Polygon.");

        var isPointInside = obj.Geometry.Contains(dto.Point);

        return new OkObjectResult(isPointInside);
    }

    public async Task<IActionResult> GetSquareCentroid(int id)
    {
        var obj = await _dbContext.Squares.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        if (obj.Geometry == null)
            return new BadRequestObjectResult("Invalid geometry. Expected Polygon.");

        // Calculate centroid
        var centroid = obj.Geometry.Centroid;

        if (centroid == null || centroid.IsEmpty)
            return new BadRequestObjectResult("Could not calculate centroid for the polygon.");

        return new OkObjectResult(centroid);
    }
}