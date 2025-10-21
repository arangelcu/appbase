using AppBase.API.Model.Dto;
using AppBase.API.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.API.Model.Repositories;

public interface ILandMarkRepository
{
    Task<ActionResult> GetAll(string? name, string? description, Pageable pageable);

    Task<ActionResult> GetAllGeoJson(string? name, string? description, Pageable pageable);

    Task<ActionResult> GetById(int id);

    Task<ActionResult> Add(LandMarkReqDto dto);

    Task<ActionResult> Upd(LandMarkReqDto dto, int id);

    Task<ActionResult> Del(int id);
}