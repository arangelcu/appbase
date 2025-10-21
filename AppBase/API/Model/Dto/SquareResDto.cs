using NetTopologySuite.Geometries;

namespace AppBase.API.Model.Dto;

public class SquareResDto
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public Polygon? Geometry { get; set; }
}