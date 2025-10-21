using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Services;

public interface IStreetService
{
    Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable);

    Task<ActionResult> GetById(int id);

    Task<ActionResult> Add(StreetReqDto dto);

    Task<ActionResult> Upd(StreetReqDto dto, int id);

    Task<ActionResult> Del(int id);
}