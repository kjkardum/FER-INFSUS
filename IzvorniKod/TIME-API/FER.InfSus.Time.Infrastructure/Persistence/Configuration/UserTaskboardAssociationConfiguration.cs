using FER.InfSus.Time.Domain;
using FER.InfSus.Time.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FER.InfSus.Time.Infrastructure.Persistence.Configuration;

public class UserTaskboardAssociationsConfiguration: IEntityTypeConfiguration<UserTaskboardAssociation>
{
    public void Configure(EntityTypeBuilder<UserTaskboardAssociation> builder)
    {
        builder
            .HasOne<User>(t => t.User)
            .WithMany(t => t.UserTaskboards)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<Taskboard>(t => t.Taskboard)
            .WithMany(t => t.TaskboardUsers)
            .HasForeignKey(t => t.TaskboardId);
    }
}
