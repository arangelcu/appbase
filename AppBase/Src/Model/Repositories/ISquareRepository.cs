using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Model.Repositories;

public interface ISquareRepository
{
    Task<ActionResult> GetAll(string? name, string? description, Pageable pageable);

    Task<ActionResult> GetAllGeoJson(string? name, string? description, Pageable pageable);

    Task<ActionResult> GetById(int id);

    Task<ActionResult> Add(SquareReqDto dto);

    Task<ActionResult> Upd(SquareReqDto dto, int id);

    Task<ActionResult> Del(int id);
}