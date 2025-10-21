using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace AppBase.API.Model.Entity;

[Table("squares", Schema = "public")]
public class Square
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required] [Column("name")] public string Name { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("capacity")] public int Capacity { get; set; }

    [Required]
    [Column("geometry", TypeName = "geometry(Polygon, 4326)")]
    public Polygon? Geometry { get; set; }

    [Required] [Column("update_at")] public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public uint Version { get; set; }
}