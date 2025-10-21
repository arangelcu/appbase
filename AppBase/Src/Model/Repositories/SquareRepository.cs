using System.Data;
using System.Linq.Dynamic.Core;
using AppBase.Config.Data;
using AppBase.Model.Dto;
using AppBase.Model.Entity;
using AppBase.Utils;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace AppBase.Model.Repositories;

public class SquareRepository : ISquareRepository
{
    private readonly ApiDbContext _dbContext;

    public SquareRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ActionResult> GetAll(string? name, string? description,
        Pageable pageable)
    {
        var query = _dbContext.Squares.AsQueryable();

        if (!string.IsNullOrEmpty(name)) query = query.Where(r => r.Name.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrEmpty(description))
            query = query.Where(r => r.Description.ToLower().Contains(description.ToLower()));

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(pageable.SortBy))
        {
            var sortExpression = pageable.SortOrder?.ToLower() == "desc"
                ? $"{pageable.SortBy} DESC"
                : pageable.SortBy;

            query = query.OrderBy(sortExpression);
        }
        else
        {
            query = query.OrderBy(wt => wt.Id);
        }

        var page = pageable.Page - 1;
        if (page < 0) page = 0;

        var allData = await query
            .Select(w => new SquareResDto
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                Capacity = w.Capacity,
                Geometry = (Polygon)w.Geometry
            })
            .Skip(page * pageable.PageSize)
            .Take(pageable.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageable.PageSize);

        var result = new Utils.Paging.PagedResult<SquareResDto>
        {
            Data = allData,
            Page = pageable.Page,
            PageSize = pageable.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };

        return new OkObjectResult(result);
    }

    public async Task<ActionResult> GetAllGeoJson(string? name, string? description, Pageable pageable)
    {
        var query = _dbContext.Squares.AsQueryable();

        if (!string.IsNullOrEmpty(name)) query = query.Where(r => r.Name.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrEmpty(description))
            query = query.Where(r => r.Description.ToLower().Contains(description.ToLower()));

        if (!string.IsNullOrEmpty(pageable.SortBy))
        {
            var sortExpression = pageable.SortOrder?.ToLower() == "desc"
                ? $"{pageable.SortBy} DESC"
                : pageable.SortBy;

            query = query.OrderBy(sortExpression);
        }
        else
        {
            query = query.OrderBy(wt => wt.Id);
        }

        var page = pageable.Page - 1;
        if (page < 0) page = 0;

        var allData = await query
            .Select(w => new Feature
            {
                Geometry = w.Geometry,
                Attributes = new AttributesTable
                {
                    { "id", w.Id },
                    { "name", w.Name },
                    { "description", w.Description },
                    { "capacity", w.Capacity }
                }
            })
            .Skip(page * pageable.PageSize)
            .Take(pageable.PageSize)
            .ToListAsync();

        var featureCollection = new FeatureCollection();
        foreach (var feature in allData) featureCollection.Add(feature);

        var writer = new GeoJsonWriter();
        var geoJson = writer.Write(featureCollection);

        return new ContentResult
        {
            Content = geoJson,
            ContentType = "application/json",
            StatusCode = 200
        };
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
            Geometry = (Polygon)dto.Geometry,
            Capacity = dto.Capacity
        };
        _dbContext.Squares.Add(obj);
        await _dbContext.SaveChangesAsync();

        return new OkObjectResult(new SquareResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<ActionResult> Upd(SquareReqDto dto, int id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Squares.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        obj.Name = dto.Name;
        obj.Description = dto.Description;
        obj.UpdateAt = DateTime.UtcNow;
        obj.Geometry = (Polygon)(dto.Geometry ?? obj.Geometry);
        obj.Capacity = obj.Capacity;

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new OkObjectResult(new SquareResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = (Polygon)obj.Geometry,
            Capacity = obj.Capacity
        });
    }

    public async Task<ActionResult> Del(int id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.Squares.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        _dbContext.Squares.Remove(obj);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return new OkObjectResult(Message.Resource_Deleted);
    }
}