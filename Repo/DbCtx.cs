using Microsoft.EntityFrameworkCore;
using Models.DTOs;

namespace Repo;

public class DbCtx(DbContextOptions<DbCtx> options) : DbContext(options)
{
    public virtual required DbSet<GameDTO> Games { get; set; }

    public virtual required DbSet<VersionDbTables> VersionDbTables { get; set; }
}