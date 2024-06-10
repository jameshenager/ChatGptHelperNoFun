using Helper.ServiceGateways.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helper.ServiceGateways.ModelBuilder;

public class QueryConfiguration : IEntityTypeConfiguration<Query>
{
    public void Configure(EntityTypeBuilder<Query> builder)
    {
        builder
            .HasOne(q => q.Response)
            .WithOne(r => r.Query)
            .HasForeignKey<Response>(r => r.ResponseId);
        builder
            .HasKey(q => q.QueryId);
        builder
            .HasOne<Response>(q => q.Response)
            .WithOne(r => r.Query)
            .HasForeignKey<Response>(r => r.ResponseId);
    }
}