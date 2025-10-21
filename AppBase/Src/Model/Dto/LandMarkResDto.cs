using NetTopologySuite.Geometries;

namespace AppBase.Model.Dto;

public class LandMarkResDto
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Point? Geometry { get; set; }
}