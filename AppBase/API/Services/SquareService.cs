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

public class SquareService : ISquareService
{
    private readonly ApiDbContext _dbContext;
    private readonly SridSettings _sridSettings;
    private readonly ISquareRepository _squareRepository;

    public SquareService(ApiDbContext dbContext, ISquareRepository squareRepository,
        IOptions<SridSettings> sridSettings)
    {
        _dbContext = dbContext;
        _squareRepository = squareRepository;
        _sridSettings = sridSettings.Value;
    }

    public async Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable)
    {
        if (geojson is true) return await _squareRepository.GetAllGeoJson(name, description, pageable);

        return await _squareRepository.GetAll(name, description, pageable);
    }

    public async Task<ActionResult> GetById(int id)
    {
        return await _squareRepository.GetById(id);
    }

    public async Task<ActionResult> Add(SquareReqDto dto)
    {
        if (dto.Srid != null && dto.Srid != _sridSettings.TargetSrid)
        {
            Polygon? geom = await _dbContext.ReprojectPolygonAsync(dto.Geometry, dto.Srid.Value,
                _sridSettings.TargetSrid);
            if (geom == null)
                return new BadRequestObjectResult(Message.Error_Reprojection);
            dto.Geometry = geom;
        }

        return await _squareRepository.Add(dto);
    }

    public async Task<ActionResult> Upd(SquareReqDto dto, int id)
    {
        if (dto.Srid != null && dto.Srid != _sridSettings.TargetSrid)
        {
            Polygon? geom = await _dbContext.ReprojectPolygonAsync(dto.Geometry, dto.Srid.Value,
                _sridSettings.TargetSrid);
            if (geom == null)
                return new BadRequestObjectResult(Message.Error_Reprojection);
            dto.Geometry = geom;
        }

        return await _squareRepository.Upd(dto, id);
    }

    public async Task<ActionResult> Del(int id)
    {
        return await _squareRepository.Del(id);
    }
}