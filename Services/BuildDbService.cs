using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Repo;

namespace Services
{
    public class BuildDbService(IDbContextFactory<DbCtx> DbCtx) : IBuildDbService
    {
        public void Init()
        {
            using var context = DbCtx.CreateDbContext();
            context.Database.EnsureCreated();

            VersionDbTables? actualVesionDbTables = context.VersionDbTables.FirstOrDefault();

            VersionDbTables newVersionDbTables = new() { Id = 0, Version = 4 };

            if (actualVesionDbTables != null)
            {
                if (actualVesionDbTables.Version != newVersionDbTables.Version)
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    actualVesionDbTables.Version = newVersionDbTables.Version;

                    context.VersionDbTables.Add(actualVesionDbTables);

                    context.SaveChanges();
                }
            }
            else
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.VersionDbTables.Add(newVersionDbTables);

                context.SaveChanges();
            }
        }
    }
}
