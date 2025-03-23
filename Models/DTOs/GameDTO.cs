namespace Models.DTOs;

[Microsoft.EntityFrameworkCore.Index(nameof(IGDBId), IsUnique = true)]
public class GameDTO : DTOBase
{    
    public int? IGDBId { get; set; }

    public int UserId { get; set; }

    public required string Name { get; set; }

    public string? ReleaseDate { get; set; }

    public string? Platforms { get; set; }

    public string? Summary { get; set; }

    public string? CoverUrl { get; set; }

    public required GameStatus Status { get; set; }

    public int? Rate { get; set; }
}

public enum GameStatus
{
    Want,
    Playing,
    Played
}