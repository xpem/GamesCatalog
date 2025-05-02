using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTOs
{
    [Table("ApiOperation")]
    public class ApiOperationDTO
    {
        [Key]
        public int Id { get; set; }

        public string? Content { get; set; }

        public required ObjectType ObjectType { get; set; }

        public required ExecutionType ExecutionType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ApiOperationStatus Status { get; set; }

        public required string ObjectId { get; set; }
    }

    public enum ApiOperationStatus { Pending, Processing, Success, Failure }

    public enum ObjectType { Game }

    public enum ExecutionType { Insert, Update, Delete }
}
