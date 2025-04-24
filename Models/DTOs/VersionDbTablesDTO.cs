using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTOs;

[Table("VersionDbTables")]
public class VersionDbTablesDTO()
{
    public required int Id { get; set; }

    public required int Version { get; set; }
}
