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
            var geom = await _dbContext.ReprojectLineStringAsync(dto.Geometry, dto.Srid.Value,
                _sridSettings.TargetSrid);
            if (geom == null)
                return new BadRequestObjectResult(Message.Error_Reprojection);
            dto.Geometry = geom;
        }

        return await _streetRepository.Add(dto);
    }

    public async Task<ActionResult> Upd(StreetReqDto dto, int id)
    {
        if (dto.Srid != null && dto.Srid != _sridSettings.TargetSrid)
        {
            var geom = await _dbContext.ReprojectLineStringAsync(dto.Geometry, dto.Srid.Value,
                _sridSettings.TargetSrid);
            if (geom == null)
                return new BadRequestObjectResult(Message.Error_Reprojection);
            dto.Geometry = geom;
        }

        return await _streetRepository.Upd(dto, id);
    }

    public async Task<ActionResult> Del(int id)
    {
        return await _streetRepository.Del(id);
    }
}