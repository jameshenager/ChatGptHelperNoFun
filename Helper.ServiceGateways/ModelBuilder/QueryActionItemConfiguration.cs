using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helper.ServiceGateways.ModelBuilder;

public class QueryActionItemConfiguration : IEntityTypeConfiguration<QueryActionItem>
{
    public void Configure(EntityTypeBuilder<QueryActionItem> builder)
    {
        builder
            .HasKey(qa => new { qa.QueryId, qa.ActionItemId, }); // Composite key

        builder
            .HasOne(qa => qa.Query)
            .WithMany(q => q.QueryActionItems)
            .HasForeignKey(qa => qa.QueryId);

        builder
            .HasOne(qa => qa.ActionItem)
            .WithMany(ai => ai.QueryActionItems)
            .HasForeignKey(qa => qa.ActionItemId);
    }
}