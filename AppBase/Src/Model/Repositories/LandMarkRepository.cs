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

public class LandMarkRepository : ILandMarkRepository
{
    private readonly ApiDbContext _dbContext;

    public LandMarkRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ActionResult> GetAll(string? name, string? description,
        Pageable pageable)
    {
        var query = _dbContext.LandMarks.AsQueryable();

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
            .Select(w => new LandMarkResDto
            {
                Id = w.Id,
                Name = w.Name,
                Description = w.Description,
                Geometry = (Point)w.Geometry
            })
            .Skip(page * pageable.PageSize)
            .Take(pageable.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageable.PageSize);

        var result = new Utils.Paging.PagedResult<LandMarkResDto>
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
        var query = _dbContext.LandMarks.AsQueryable();

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
                    { "description", w.Description }
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
        var obj = await _dbContext.LandMarks
            .Where(r => r.Id == id)
            .Select(r => new LandMarkResDto
            {
                Name = r.Name,
                Description = r.Description,
                Geometry = (Point)r.Geometry
            })
            .FirstOrDefaultAsync();
        return obj == null ? new NotFoundObjectResult(Message.Warning_NotFound) : new OkObjectResult(obj);
    }

    public async Task<ActionResult> Add(LandMarkReqDto dto)
    {
        var obj = new LandMark
        {
            Name = dto.Name,
            Description = dto.Description,
            UpdateAt = DateTime.UtcNow,
            Geometry = (Point)dto.Geometry
        };
        _dbContext.LandMarks.Add(obj);
        await _dbContext.SaveChangesAsync();

        return new OkObjectResult(new LandMarkResDto
        {
            Id = obj.Id,
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry
        });
    }

    public async Task<ActionResult> Upd(LandMarkReqDto dto, int id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.LandMarks.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        obj.Name = dto.Name;
        obj.Description = dto.Description;
        obj.UpdateAt = DateTime.UtcNow;
        obj.Geometry = (Point)(dto.Geometry ?? obj.Geometry);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new OkObjectResult(new LandMarkResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = (Point)obj.Geometry
        });
    }

    public async Task<ActionResult> Del(int id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.LandMarks.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        _dbContext.LandMarks.Remove(obj);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return new OkObjectResult(Message.Resource_Deleted);
    }
}