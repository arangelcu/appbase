using AppBase.Config.Filters;
using AppBase.Model.Dto;
using AppBase.Services;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Controller;

[ApiController]
[Route("[controller]")]
[Metrics]
public class StreetController : ControllerBase
{
    private readonly IStreetService _streetService;

    public StreetController(IStreetService streetService)
    {
        _streetService = streetService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? description,
        [FromQuery] bool? geojson,
        [FromQuery] Pageable pageable)
    {
        return await _streetService.GetAll(name, description, geojson, pageable);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return await _streetService.GetById(id);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] StreetReqDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _streetService.Add(dto);
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<IActionResult> Upd([FromRoute] int id, [FromBody] StreetReqDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _streetService.Upd(dto, id);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Del([FromRoute] int id)
    {
        return await _streetService.Del(id);
    }

    //Remove Closest Point
    [HttpPut]
    [Route("{id:int}/remove_closest")]
    public async Task<IActionResult> DelPointFromStreet(
        [FromRoute] int id,
        [FromBody] GeometryUpdDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _streetService.DelPointFromStreet(id, dto);
    }

    //Extend Line
    [HttpPut]
    [Route("{id:int}/add_point")]
    public async Task<IActionResult> AddPointToStreet(
        [FromRoute] int id,
        [FromBody] GeometryUpdDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _streetService.AddPointToStreet(id, dto);
    }
}