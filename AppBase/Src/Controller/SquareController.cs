using AppBase.Config.Filters;
using AppBase.Model.Dto;
using AppBase.Services;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace AppBase.Controller;

[ApiController]
[Route("[controller]")]
[Metrics]
public class SquareController : ControllerBase
{
    private readonly ISquareService _squareService;

    public SquareController(ISquareService squareService)
    {
        _squareService = squareService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? description,
        [FromQuery] bool? geojson,
        [FromQuery] Pageable pageable)
    {
        return await _squareService.GetAll(name, description, geojson, pageable);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return await _squareService.GetById(id);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] SquareReqDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _squareService.Add(dto);
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<IActionResult> Upd(
        [FromRoute] int id,
        [FromBody] SquareReqDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _squareService.Upd(dto, id);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Del([FromRoute] int id)
    {
        return await _squareService.Del(id);
    }

    [HttpPut]
    [Route("{id:int}/add_point")]
    public async Task<IActionResult> AddPointToSquare(
        [FromRoute] int id,
        [FromBody] GeometryUpdDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _squareService.AddPointToSquare(id, dto);
    }

    [HttpGet]
    [Route("{id:int}/contain_point")]
    public async Task<IActionResult> CheckSquareContainPoint(
        [FromRoute] int id,
        [FromBody] GeometryUpdDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _squareService.CheckSquareContainPoint(id, dto);
    }

    [HttpGet]
    [Route("{id:int}/centroid")]
    public async Task<IActionResult> GetSquareCentroid([FromRoute] int id)
    {
        return await _squareService.GetSquareCentroid(id);
    }
}