using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolMS.Domain.Common;

namespace SchoolMS.Infrastructure.Data.Interceptors;



public class AuditableEntityInterceptor(TimeProvider dateTime) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (EntityEntry<AuditableEntity> entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                DateTimeOffset utcNow = dateTime.GetUtcNow();
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDateUtc = utcNow;
                }


                foreach (ReferenceEntry ownedEntry in entry.References)
                {
                    if (ownedEntry.TargetEntry is { Entity: AuditableEntity ownedEntity }
                        && ownedEntry.TargetEntry.State is EntityState.Added or EntityState.Modified)
                    {
                        if (ownedEntry.TargetEntry.State == EntityState.Added)
                        {
                            ownedEntity.CreatedDateUtc = utcNow;
                        }

                    }
                }
            }
        }
    }
}
public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry?.Metadata.IsOwned() == true &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}