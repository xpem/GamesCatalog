using System.ComponentModel.DataAnnotations;

namespace Models.DTOs;

public class DTOBase
{
    [Key]
    public int Id { get; set; }

    public int? ExternalId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool Inactive { get; set; }
}
