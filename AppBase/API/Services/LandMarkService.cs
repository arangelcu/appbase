using AppBase.API.Config.Data;
using AppBase.API.Config.Extensions;
using AppBase.API.Config.Srid;
using AppBase.API.Model.Dto;
using AppBase.API.Model.Repositories;
using AppBase.API.Utils;
using AppBase.API.Utils.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace AppBase.API.Services;

public class LandMarkService : ILandMarkService
{
    private readonly ApiDbContext _dbContext;
    private readonly SridSettings _sridSettings;
    private readonly ILandMarkRepository _landMarkRepository;

    public LandMarkService(ApiDbContext dbContext, ILandMarkRepository landMarkRepository,
        IOptions<SridSettings> sridSettings)
    {
        _dbContext = dbContext;
        _landMarkRepository = landMarkRepository;
        _sridSettings = sridSettings.Value;
    }

    public async Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable)
    {
        if (geojson is true) return await _landMarkRepository.GetAllGeoJson(name, description, pageable);

        return await _landMarkRepository.GetAll(name, description, pageable);
    }

    public async Task<ActionResult> GetById(int id)
    {
        return await _landMarkRepository.GetById(id);
    }

    public async Task<ActionResult> Add(LandMarkReqDto dto)
    {
        if (dto.Srid != null && dto.Srid != _sridSettings.TargetSrid)
        {
            Point? geom = await _dbContext.ReprojectPointAsync(dto.Geometry, dto.Srid.Value,
                _sridSettings.TargetSrid);
            if (geom == null)
                return new BadRequestObjectResult(Message.Error_Reprojection);
            dto.Geometry = geom;
        }

        return await _landMarkRepository.Add(dto);
    }

    public async Task<ActionResult> Upd(LandMarkReqDto dto, int id)
    {
        if (dto.Srid != null && dto.Srid != _sridSettings.TargetSrid)
        {
            Point? geom = await _dbContext.ReprojectPointAsync(dto.Geometry, dto.Srid.Value,
                _sridSettings.TargetSrid);
            if (geom == null)
                return new BadRequestObjectResult(Message.Error_Reprojection);
            dto.Geometry = geom;
        }

        return await _landMarkRepository.Upd(dto, id);
    }

    public async Task<ActionResult> Del(int id)
    {
        return await _landMarkRepository.Del(id);
    }
}