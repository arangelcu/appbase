using System.Data;
using System.Diagnostics;
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

public class StreetService : IStreetService
{
    private readonly ApiDbContext _dbContext;
    private readonly IStreetRepository _streetRepository;

    public StreetService(ApiDbContext dbContext, IStreetRepository streetRepository)
    {
        _dbContext = dbContext;
        _streetRepository = streetRepository;
    }

    public async Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable)
    {
        var obj = await _streetRepository.GetAll(name, description, pageable);

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
        var obj = await _dbContext.Streets
            .Where(r => r.Id == id)
            .Select(r => new StreetResDto
            {
                Name = r.Name,
                Description = r.Description,
                Geometry = (LineString)r.Geometry,
                Capacity = r.Capacity
            })
            .FirstOrDefaultAsync();
        return obj == null ? new NotFoundObjectResult(Message.Warning_NotFound) : new OkObjectResult(obj);
    }

    public async Task<ActionResult> Add(StreetReqDto dto)
    {
        var obj = new Street
        {
            Name = dto.Name,
            Description = dto.Description,
            UpdateAt = DateTime.UtcNow,
            Geometry = dto.Geometry,
            Capacity = dto.Capacity
        };
        _dbContext.Streets.Add(obj);
        await _dbContext.SaveChangesAsync();

        return new OkObjectResult(new StreetResDto
        {
            Id = obj.Id,
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<ActionResult> Upd(StreetReqDto dto, int id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Streets.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        obj.Name = dto.Name;
        obj.Description = dto.Description;
        obj.UpdateAt = DateTime.UtcNow;
        obj.Geometry = dto.Geometry;
        obj.Capacity = obj.Capacity;

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new OkObjectResult(new StreetResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<ActionResult> Del(int id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Streets.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        _dbContext.Streets.Remove(obj);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return new OkObjectResult(Message.Resource_Deleted);
    }

    public async Task<IActionResult> DelPointFromStreet(int id, GeometryUpdDto dto)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Streets.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null)
            return new NotFoundObjectResult(Message.Warning_NotFound);

        if (obj.Geometry == null)
            return new BadRequestObjectResult("Invalid geometry. Expected LineString.");

        if (obj.Geometry.Coordinates.Length < 3)
            return new BadRequestObjectResult("Line must have at least 3 points to remove one.");

        if (dto.Postgis is true)
        {
        }
        else
        {
            obj.Geometry = LineStringUtils.RemoveClosestLinestringPointFromReferencePoint(obj.Geometry, dto.Point);
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return new OkObjectResult(new StreetResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<IActionResult> AddPointToStreet(int id, GeometryUpdDto dto)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Streets.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        if (obj.Geometry == null)
            return new BadRequestObjectResult("Invalid geometry. Expected LineString.");

        if (dto.Postgis is true)
        {
            obj.Geometry =
                await LineStringUtils.AddPointToClosestLinestringEndpointPostGis(_dbContext, obj.Geometry,
                    dto.Point);
        }
        else
        {
            obj.Geometry = LineStringUtils.AddPointToClosestLinestringEndpoint(obj.Geometry, dto.Point);
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return new OkObjectResult(new StreetResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<IActionResult> SmoothStreet(int id, GeometrySmoothDto dto)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Streets.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        if (obj.Geometry == null)
            return new BadRequestObjectResult("Invalid geometry. Expected LineString.");

        if (dto.Postgis is true)
        {
        }
        else
        {
            obj.Geometry = LineStringUtils.ApplyBezierSmoothingToLinestring(obj.Geometry, dto.Intensity);
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return new OkObjectResult(new StreetResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }
}