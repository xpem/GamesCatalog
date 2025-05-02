using Models.DTOs;

namespace Models.Resps.Api
{
    public record GameStatusApiResp
    {
        public int Id { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required GameStatus Status { get; set; }

        public int? Rate { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Inactive { get; set; }

        public GameApiResp? Game { get; set; }
    }

    public record GameApiResp
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? IGDBId { get; set; }

        public string? Name { get; set; }

        public string? ReleaseDate { get; set; }

        public string? Platforms { get; set; }

        public string? Summary { get; set; }

        public string? CoverUrl { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
