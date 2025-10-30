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
    [ValidateModel]
    public async Task<IActionResult> Add([FromBody] SquareReqDto dto)
    {
        return await _squareService.Add(dto);
    }

    [HttpPut]
    [Route("{id:int}")]
    [ValidateModel]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] SquareReqDto dto)
    {
        return await _squareService.Update(id, dto);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return await _squareService.Delete(id);
    }

    [HttpPut]
    [Route("{id:int}/add_point")]
    [ValidateModel]
    public async Task<IActionResult> AddPointToSquare(
        [FromRoute] int id,
        [FromBody] GeometryUpdDto dto)
    {
        return await _squareService.AddPointToSquare(id, dto);
    }

    [HttpPost]
    [Route("{id:int}/contain_point")]
    [ValidateModel]
    public async Task<IActionResult> CheckSquareContainPoint(
        [FromRoute] int id,
        [FromBody] GeometryUpdDto dto)
    {
        return await _squareService.CheckSquareContainPoint(id, dto);
    }

    [HttpGet]
    [Route("{id:int}/centroid")]
    public async Task<IActionResult> GetSquareCentroid([FromRoute] int id)
    {
        return await _squareService.GetSquareCentroid(id);
    }
}