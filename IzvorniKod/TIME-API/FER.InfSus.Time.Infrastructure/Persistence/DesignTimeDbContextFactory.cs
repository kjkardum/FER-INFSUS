using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FER.InfSus.Time.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private const string DesignTimeConnectionString
        = "Server=localhost,1433;Database=FER.InfSus.Time.Backend;MultipleActiveResultSets=true;User=sa;Password=Passw0rd_;Encrypt=false";

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(DesignTimeConnectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
