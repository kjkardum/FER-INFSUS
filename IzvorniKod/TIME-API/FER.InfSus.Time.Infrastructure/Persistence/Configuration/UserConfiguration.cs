using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Infrastructure.Persistence.Configuration;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Email).IsUnique();

        builder.Property(t => t.Email).HasColumnType("nvarchar(100)");
        builder.Property(t => t.PasswordHash).HasColumnType("nvarchar(100)");
    }
}
