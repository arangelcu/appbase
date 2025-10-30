using AppBase.Config.Filters;
using AppBase.Model.Dto;
using AppBase.Services;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Http.HttpResults;
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
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] string? description,
        [FromQuery] bool? geojson, [FromQuery] Pageable pageable)
    {
        return await _squareService.GetAll(name, description, geojson, pageable);
    }

    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        return await _squareService.GetById(id);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Add(SquareReqDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _squareService.Add(dto);
    }

    [HttpPut]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upd(SquareReqDto dto, int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _squareService.Upd(dto, id);
    }

    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Del(int id)
    {
        return await _squareService.Del(id);
    }

    [HttpPut]
    [Route("{id:int}/add_point")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddPointToSquare(int id, GeometryUpdDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _squareService.AddPointToSquare(id, dto);
    }

    [HttpGet]
    [Route("{id:int}/contain_point")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckSquareContainPoint(int id, GeometryUpdDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _squareService.CheckSquareContainPoint(id, dto);
    }

    [HttpGet]
    [Route("{id:int}/centroid")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSquareCentroid(int id)
    {
        return await _squareService.GetSquareCentroid(id);
    }
}