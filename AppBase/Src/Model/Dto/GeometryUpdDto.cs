using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace AppBase.Model.Dto;

public class GeometryUpdDto
{
    [Required(ErrorMessage = "Field is required")]
    public Point Point { get; set; }

    public bool? Postgis { get; set; }
}