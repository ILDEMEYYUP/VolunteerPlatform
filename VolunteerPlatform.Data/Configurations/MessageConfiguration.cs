using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VolunteerPlatform.Domain.Entities;

namespace VolunteerPlatform.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        // Primary Key
        builder.HasKey(m => m.Id);

        // Content
        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);

        // SentAt
        builder.Property(m => m.SentAt)
            .IsRequired();

        // Status
        builder.Property(m => m.Status)
            .IsRequired();

        // Sender Relation
        // MSSQL'de "multiple cascade paths" hatasını önlemek için OnDelete(DeleteBehavior.Restrict) ekledim.
        builder.HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Receiver Relation
        builder.HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
