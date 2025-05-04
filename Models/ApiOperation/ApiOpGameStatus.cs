using Models.DTOs;

namespace Models.ApiOperation
{
    public class ApiOpGameStatus(int id, GameStatus? status, int? rate)
    {
        public int Id { get; set; } = id;

        public GameStatus? Status { get; set; } = status;

        public int? Rate { get; set; } = rate;
    }
}
