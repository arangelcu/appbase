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
public class LandMarkController : ControllerBase
{
    private readonly ILandMarkService _landMarkService;

    public LandMarkController(ILandMarkService landMarkService)
    {
        _landMarkService = landMarkService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] string? description,
        [FromQuery] bool? geojson, [FromQuery] Pageable pageable)
    {
        return await _landMarkService.GetAll(name, description, geojson, pageable);
    }

    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        return await _landMarkService.GetById(id);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Add(LandMarkReqDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _landMarkService.Add(dto);
    }

    [HttpPut]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upd(LandMarkReqDto dto, int id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _landMarkService.Upd(dto, id);
    }

    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(OkObjectResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Del(int id)
    {
        return await _landMarkService.Del(id);
    }
}