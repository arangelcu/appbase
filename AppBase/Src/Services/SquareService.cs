using AppBase.Config.Data;
using AppBase.Config.Extensions;
using AppBase.Config.Srid;
using AppBase.Model.Dto;
using AppBase.Model.Repositories;
using AppBase.Utils;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AppBase.Services;

public class SquareService : ISquareService
{
    private readonly ApiDbContext _dbContext;
    private readonly ISquareRepository _squareRepository;
    private readonly SridSettings _sridSettings;

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
            var geom = await _dbContext.ReprojectPolygonAsync(dto.Geometry, dto.Srid.Value,
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
            var geom = await _dbContext.ReprojectPolygonAsync(dto.Geometry, dto.Srid.Value,
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