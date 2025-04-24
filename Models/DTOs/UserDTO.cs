using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTOs;

[Table("User")]
public class UserDTO
{
    public required int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Token { get; set; }

    public DateTime LastUpdate { get; set; }
}
