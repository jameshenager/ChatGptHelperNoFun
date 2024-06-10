using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helper.ServiceGateways.ModelBuilder;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasMany(c => c.Queries)
            .WithOne(q => q.Category)
            .HasForeignKey(q => q.CategoryId);

        builder
            .HasMany(c => c.Queries)
            .WithOne(q => q.Category)
            .HasForeignKey(q => q.CategoryId);
    }
}