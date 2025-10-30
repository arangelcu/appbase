using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace AppBase.Model.Dto;

public class LandMarkReqDto
{
    [Required(ErrorMessage = "Field is required")]
    public string Name { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public Point Geometry { get; set; }
}