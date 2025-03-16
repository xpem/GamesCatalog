namespace Models.DTOs;

public record VersionDbTables()
{
    public required int Id { get; set; }

    public required int Version { get; set; }
}
