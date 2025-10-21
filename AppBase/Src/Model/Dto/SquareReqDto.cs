using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace AppBase.Model.Dto;

public class SquareReqDto
{
    [Required(ErrorMessage = "Field is required")]
    public string Name { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public int Capacity { get; set; }

    public int? Srid { get; set; }

    [Required(ErrorMessage = "Field is required")]
    public Polygon Geometry { get; set; }
}