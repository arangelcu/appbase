using System.Data;
using AppBase.Config.Data;
using AppBase.Model.Dto;
using AppBase.Model.Entity;
using AppBase.Model.Repositories;
using AppBase.Utils;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace AppBase.Services;

public class LandMarkService : ILandMarkService
{
    private readonly ApiDbContext _dbContext;
    private readonly ILandMarkRepository _landMarkRepository;

    public LandMarkService(ApiDbContext dbContext, ILandMarkRepository landMarkRepository)
    {
        _dbContext = dbContext;
        _landMarkRepository = landMarkRepository;
    }

    public async Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable)
    {
        var obj = await _landMarkRepository.GetAll(name, description, pageable);

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
                        { "description", item.Description }
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
        var obj = await _dbContext.LandMarks
            .Where(r => r.Id == id)
            .Select(r => new LandMarkResDto
            {
                Name = r.Name,
                Description = r.Description,
                Geometry = r.Geometry
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
            Geometry = dto.Geometry
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

    public async Task<ActionResult> Update(int id, LandMarkReqDto dto)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var obj = await _dbContext.LandMarks.FirstOrDefaultAsync(r => r.Id == id);
        if (obj == null) return new NotFoundObjectResult(Message.Warning_NotFound);

        obj.Name = dto.Name;
        obj.Description = dto.Description;
        obj.UpdateAt = DateTime.UtcNow;
        obj.Geometry = dto.Geometry;

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new OkObjectResult(new LandMarkResDto
        {
            Name = obj.Name,
            Description = obj.Description,
            Geometry = obj.Geometry
        });
    }

    public async Task<ActionResult> Delete(int id)
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