using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace AppBase.Model.Dto;

public class GeometrySmoothDto
{
    [Required(ErrorMessage = "Field is required")]
    public double Intensity { get; set; }
    
    public bool? Postgis { get; set; }
}