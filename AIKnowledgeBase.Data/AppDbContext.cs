using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AIKnowledgeBase.Core.Entities;

namespace AIKnowledgeBase.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public DbSet<ChatMessage> ChatMessages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Bu satırı unutma, base sınıftaki ayarları korur.

        modelBuilder.Entity<DocumentTag>()
            .HasKey(dt => new { dt.DocumentId, dt.TagId }); // Kompozit Anahtar tanımlaması

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Document)
            .WithMany(d => d.DocumentTags)
            .HasForeignKey(dt => dt.DocumentId)
            .OnDelete(DeleteBehavior.Cascade); // Belge silindiğinde ilişkili DocumentTag kayıtlarını da sil

        modelBuilder.Entity<DocumentTag>()
            .HasOne(dt => dt.Tag)
            .WithMany(t => t.DocumentTags)
            .HasForeignKey(dt => dt.TagId)
            .OnDelete(DeleteBehavior.Restrict); // Tag silindiğinde ilişkili DocumentTag kayıtlarını silme, çünkü bir tag birden fazla belgeye ait olabilir

    }


}



