using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Services;

public interface ISquareService
{
    Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable);

    Task<ActionResult> GetById(int id);

    Task<ActionResult> Add(SquareReqDto dto);

    Task<ActionResult> Update(int id, SquareReqDto dto);

    Task<ActionResult> Delete(int id);

    Task<IActionResult> AddPointToSquare(int id, GeometryUpdDto dto);

    Task<IActionResult> CheckSquareContainPoint(int id, GeometryUpdDto dto);

    Task<IActionResult> GetSquareCentroid(int id);
}