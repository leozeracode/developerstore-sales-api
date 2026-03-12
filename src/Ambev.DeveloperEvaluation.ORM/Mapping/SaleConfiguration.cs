using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
        builder.Property(s => s.SaleDate).IsRequired();
        builder.Property(s => s.IsCancelled).IsRequired();

        builder.OwnsOne(s => s.Customer, cb =>
        {
            cb.Property(c => c.Id).HasColumnName("CustomerId").IsRequired();
            cb.Property(c => c.Name).HasColumnName("CustomerName").HasMaxLength(100).IsRequired();
        });

        builder.OwnsOne(s => s.Branch, bb =>
        {
            bb.Property(b => b.Id).HasColumnName("BranchId").IsRequired();
            bb.Property(b => b.Name).HasColumnName("BranchName").HasMaxLength(100).IsRequired();
        });

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}