using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Services;

public interface IStreetService
{
    Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable);

    Task<ActionResult> GetById(int id);

    Task<ActionResult> Add(StreetReqDto dto);

    Task<ActionResult> Update(int id, StreetReqDto dto);

    Task<ActionResult> Delete(int id);

    Task<IActionResult> DelPointFromStreet(int id, GeometryUpdDto dto);

    Task<IActionResult> AddPointToStreet(int id, GeometryUpdDto dto);
}