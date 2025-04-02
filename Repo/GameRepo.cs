﻿using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;

namespace Repo
{
    public class GameRepo(IDbContextFactory<DbCtx> DbCtx) : IGameRepo
    {
        public async Task CreateAsync(GameDTO game)
        {
            using var context = DbCtx.CreateDbContext();

            await context.Games.AddAsync(game);

            await context.SaveChangesAsync();
        }

        public async Task<GameDTO?> GetByIGDBIdAsync(int igdbId)
        {
            using var context = DbCtx.CreateDbContext();
            return await context.Games.FirstOrDefaultAsync(g => g.IGDBId == igdbId);
        }

        public List<TotalGroupedByStatus>? GetTotalsGroupedByStatusAsync(int uid)
        {
            using var context = DbCtx.CreateDbContext();

            var result = context.Games
                .Where(x => x.UserId.Equals(uid) && x.Inactive == false)
                .GroupBy(x => x.Status)
                .Select(x => new TotalGroupedByStatus
                {
                    Status = x.Key,
                    Total = x.Count(),
                    LastFiveIGDBIdsByUpdatedAt = x.OrderByDescending(g => g.UpdatedAt)
                                                    .Take(5)
                                                    .Select(g => g.IGDBId.ToString())
                                                    .ToArray()
                }).AsEnumerable();

            return [.. result];
        }

        public async Task InactivateAsync(int uid, int id, DateTime updatedAt)
        {
            using var context = DbCtx.CreateDbContext();
            await context.Games.Where(x => x.Id == id && x.UserId == uid)
                .ExecuteUpdateAsync(y => y
             .SetProperty(z => z.UpdatedAt, updatedAt)
             .SetProperty(z => z.Inactive, true));
        }

        public async Task UpdateStatusAsync(int id, DateTime updatedAt, GameStatus status, int? rate)
        {
            using var context = DbCtx.CreateDbContext();

            await context.Games.Where(x => x.Id == id).ExecuteUpdateAsync(y => y
             .SetProperty(z => z.Inactive, false)
             .SetProperty(z => z.UpdatedAt, updatedAt)
             .SetProperty(z => z.Status, status)
             .SetProperty(z => z.Rate, rate));
        }

        readonly int pageSize = 10;

        public async Task<List<GameDTO>> GetByStatusAsync(int uid, GameStatus gameStatus, int page)
        {
            using var context = DbCtx.CreateDbContext();
            return await context.Games
                .Where(x => x.UserId.Equals(uid) && x.Status == gameStatus && x.Inactive == false)
                .OrderByDescending(x => x.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<GameDTO>> GetByStatusAsync(int uid, GameStatus gameStatus, int page, string searchText)
        {
            using var context = DbCtx.CreateDbContext();
            return await context.Games
                .Where(x => x.UserId.Equals(uid) && x.Status == gameStatus && x.Inactive == false && EF.Functions.Like(x.Name, $"%{searchText}%"))
                .OrderByDescending(x => x.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();
        }
    }
}
