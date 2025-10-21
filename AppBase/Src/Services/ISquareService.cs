using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Services;

public interface ISquareService
{
    Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable);

    Task<ActionResult> GetById(int id);

    Task<ActionResult> Add(SquareReqDto dto);

    Task<ActionResult> Upd(SquareReqDto dto, int id);

    Task<ActionResult> Del(int id);
}