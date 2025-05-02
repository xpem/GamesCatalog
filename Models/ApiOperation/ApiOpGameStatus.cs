using Models.DTOs;

namespace Models.ApiOperation
{
    public class ApiOpGameStatus(int userId, GameStatus? status, int? rate) : DTOBase
    {
        public int UserId { get; set; } = userId;

        public GameStatus? Status { get; set; } = status;

        public int? Rate { get; set; } = rate;
    }
}
