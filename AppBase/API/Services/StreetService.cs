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

public class StreetService : IStreetService
{
    private readonly ApiDbContext _dbContext;
    private readonly SridSettings _sridSettings;
    private readonly IStreetRepository _streetRepository;

    public StreetService(ApiDbContext dbContext, IStreetRepository streetRepository,
        IOptions<SridSettings> sridSettings)
    {
        _dbContext = dbContext;
        _streetRepository = streetRepository;
        _sridSettings = sridSettings.Value;
    }

    public async Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable)
    {
        if (geojson is true) return await _streetRepository.GetAllGeoJson(name, description, pageable);

        return await _streetRepository.GetAll(name, description, pageable);
    }

    public async Task<ActionResult> GetById(int id)
    {
        return await _streetRepository.GetById(id);
    }

    public async Task<ActionResult> Add(StreetReqDto dto)
    {
        if (dto.Srid != null && dto.Srid != _sridSettings.TargetSrid)
        {
            dto.Geometry =
                (LineString)await _dbContext.ReprojectLineStringAsync(dto.Geometry, dto.Srid.Value,
                    _sridSettings.TargetSrid);
            if (dto.Geometry == null)
                return new BadRequestObjectResult(Message.Error_Reprojection);
        }

        return await _streetRepository.Add(dto);
    }

    public async Task<ActionResult> Upd(StreetReqDto dto, int id)
    {
        if (dto.Srid != null && dto.Srid != _sridSettings.TargetSrid)
        {
            dto.Geometry =
                (LineString)await _dbContext.ReprojectLineStringAsync(dto.Geometry, dto.Srid.Value,
                    _sridSettings.TargetSrid);
            if (dto.Geometry == null)
                return new BadRequestObjectResult(Message.Error_Reprojection);
        }

        return await _streetRepository.Upd(dto, id);
    }

    public async Task<ActionResult> Del(int id)
    {
        return await _streetRepository.Del(id);
    }
}