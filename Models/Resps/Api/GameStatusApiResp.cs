using Models.DTOs;
using System.Text.Json.Serialization;

namespace Models.Resps.Api
{
    public record GameStatusApiResp
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("createdAt")]
        public required DateTime CreatedAt { get; set; }

        [JsonPropertyName("status")]
        public required GameStatus Status { get; set; }

        [JsonPropertyName("rate")]
        public int? Rate { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("inactive")]
        public bool Inactive { get; set; }

        [JsonPropertyName("game")]
        public GameApiResp? Game { get; set; }
    }

    public record GameApiResp
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("igdbId")]
        public int? IGDBId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("releaseDate")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("platforms")]
        public string? Platforms { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("coverUrl")]
        public string? CoverUrl { get; set; }

        [JsonPropertyName("status")]
        public DateTime UpdatedAt { get; set; }
    }
}
