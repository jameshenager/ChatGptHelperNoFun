using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helper.ServiceGateways.ModelBuilder;

#pragma warning disable CS8618
public class ActionItemConfiguration : IEntityTypeConfiguration<ActionItem>
{
    public void Configure(EntityTypeBuilder<ActionItem> builder)
    {
        builder
            .HasKey(ai => ai.ActionItemId);

        builder
            .HasOne(ai => ai.ActionItemSchedule)
            .WithOne(ais => ais.ActionItem)
            .HasForeignKey<ActionItemSchedule>(ais => ais.ActionItemScheduleId);

        builder
            .Property(ai => ai.Description)
            .HasMaxLength(255);
    }
}