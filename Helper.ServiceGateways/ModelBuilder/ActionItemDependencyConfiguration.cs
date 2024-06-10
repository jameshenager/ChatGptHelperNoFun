using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helper.ServiceGateways.ModelBuilder;

public class ActionItemDependencyConfiguration : IEntityTypeConfiguration<ActionItemDependency>
{
    public void Configure(EntityTypeBuilder<ActionItemDependency> builder)
    {
        builder
            .HasKey(aid => new { aid.ActionItemId, aid.DependsOnActionItemId, }); // Composite key

        builder
            .HasOne(aid => aid.ActionItem)
            .WithMany(ai => ai.Dependencies) // This indicates the collection of dependencies where the ActionItem is the dependent
            .HasForeignKey(aid => aid.ActionItemId)
            .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary

        builder
            .HasOne(aid => aid.DependsOnActionItem)
            .WithMany(ai => ai.DependedOnBy) // This indicates the collection of dependencies where the ActionItem is the dependency
            .HasForeignKey(aid => aid.DependsOnActionItemId)
            .OnDelete(DeleteBehavior.Restrict); // Adjust as necessary
    }
}