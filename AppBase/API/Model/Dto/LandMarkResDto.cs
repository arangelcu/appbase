using NetTopologySuite.Geometries;

namespace AppBase.API.Model.Dto;

public class LandMarkResDto
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Point? Geometry { get; set; }
}