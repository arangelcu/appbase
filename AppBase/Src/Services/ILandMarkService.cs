using AppBase.Model.Dto;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Services;

public interface ILandMarkService
{
    Task<ActionResult> GetAll(string? name, string? description, bool? geojson, Pageable pageable);

    Task<ActionResult> GetById(int id);

    Task<ActionResult> Add(LandMarkReqDto dto);

    Task<ActionResult> Update(int id, LandMarkReqDto dto);

    Task<ActionResult> Delete(int id);
}