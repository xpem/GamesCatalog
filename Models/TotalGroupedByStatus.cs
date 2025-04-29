using Models.DTOs;

namespace Models;

public record TotalGroupedByStatus
{
    public int Total { get; set; }

    public GameStatus? Status { get; set; }

    public string?[] LastFiveIGDBIdsByUpdatedAt { get; set; } = [];
}
