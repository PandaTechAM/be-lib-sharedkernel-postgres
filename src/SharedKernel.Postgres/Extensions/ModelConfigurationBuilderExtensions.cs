using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Postgres.Extensions;

/// <summary>EF Core model-configuration helpers for the Postgres shared kernel.</summary>
public static class DbContextExtensions
{
    /// <summary>Configure every <see cref="decimal" /> property in the model with precision 40 and scale 20.</summary>
    public static ModelConfigurationBuilder ConfigureDecimalType(
        this ModelConfigurationBuilder modelConfigurationBuilder)
    {
        modelConfigurationBuilder.Properties<decimal>(builder => builder.HavePrecision(40, 20));

        return modelConfigurationBuilder;
    }

    /// <summary>Set every foreign key to <see cref="DeleteBehavior.Restrict" />, disabling cascade deletes by default.</summary>
    public static ModelBuilder RestrictFkDeleteBehaviorByDefault(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        return modelBuilder;
    }
}
