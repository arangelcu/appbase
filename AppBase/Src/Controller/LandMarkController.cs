using AppBase.Config.Filters;
using AppBase.Model.Dto;
using AppBase.Services;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Controller;

[ApiController]
[Route("[controller]")]
[Metrics]
public class LandMarkController : ControllerBase
{
    private readonly ILandMarkService _landMarkService;

    public LandMarkController(ILandMarkService landMarkService)
    {
        _landMarkService = landMarkService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? description,
        [FromQuery] bool? geojson,
        [FromQuery] Pageable pageable)
    {
        return await _landMarkService.GetAll(name, description, geojson, pageable);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return await _landMarkService.GetById(id);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Add([FromBody] LandMarkReqDto dto)
    {
        return await _landMarkService.Add(dto);
    }

    [HttpPut]
    [Route("{id:int}")]
    [ValidateModel]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] LandMarkReqDto dto)
    {
        return await _landMarkService.Update(id, dto);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return await _landMarkService.Delete(id);
    }
}