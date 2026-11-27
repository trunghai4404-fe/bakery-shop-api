using Contents.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contents.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");
        builder.HasKey(contact => contact.Id);
        builder.Property(contact => contact.Value).IsRequired().HasMaxLength(Contact.MaxValueLength);
        builder.Property(contact => contact.IsDeleted).HasDefaultValue(false);
        builder.HasQueryFilter(contact => !contact.IsDeleted);
    }
}
