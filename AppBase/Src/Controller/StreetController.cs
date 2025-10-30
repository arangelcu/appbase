using AppBase.Config.Filters;
using AppBase.Model.Dto;
using AppBase.Services;
using AppBase.Utils.Paging;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

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
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] string? description,
        [FromQuery] bool? geojson, [FromQuery] Pageable pageable)
    {
        return await _streetService.GetAll(name, description, geojson, pageable);
    }

    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        return await _streetService.GetById(id);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Add(StreetReqDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _streetService.Add(dto);
    }

    [HttpPut]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upd(StreetReqDto dto, int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _streetService.Upd(dto, id);
    }

    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Del(int id)
    {
        return await _streetService.Del(id);
    }

    //Remove Closest Point
    [HttpPut]
    [Route("{id:int}/remove_closest")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DelPointFromStreet(int id, GeometryUpdDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _streetService.DelPointFromStreet(id, dto);
    }

    //Extend Line
    [HttpPut]
    [Route("{id:int}/add_point")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddPointToStreet(int id, GeometryUpdDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _streetService.AddPointToStreet(id, dto);
    }
}