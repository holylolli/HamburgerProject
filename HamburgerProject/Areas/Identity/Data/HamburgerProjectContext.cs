using HamburgerProject.Areas.Identity.Data;
using HamburgerProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HamburgerProject.Data;

public class HamburgerProjectContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Extra> Extras { get; set; }
    public DbSet<Menu> Menus { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderExtra> OrderExtras { get; set; }

    public HamburgerProjectContext(DbContextOptions<HamburgerProjectContext> options): base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Price).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(m => m.Description).HasMaxLength(500);
            entity.Property(m => m.ImagePath).HasMaxLength(250);

            entity.HasMany(m => m.Orders)
                  .WithOne(o => o.Menu)
                  .HasForeignKey(o => o.MenuId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrderDate).IsRequired();
            entity.Property(o => o.Quantity).IsRequired();
            entity.Property(o => o.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");

            // ApplicationUser ile ilişkiyi kuruyoruz
            entity.HasOne(o => o.ApplicationUser)
                  .WithMany()
                  .HasForeignKey(o => o.ApplicationUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Menu ile ilişkiyi kuruyoruz
            entity.HasOne(o => o.Menu)
                  .WithMany(m => m.Orders)
                  .HasForeignKey(o => o.MenuId)
                  .OnDelete(DeleteBehavior.Restrict);

            // OrderExtra ile ilişkiyi kuruyoruz
            entity.HasMany(o => o.OrderExtras)
                  .WithOne(oe => oe.Order)
                  .HasForeignKey(oe => oe.OrderId) // OrderId foreign key olarak kullanılıyor
                  .OnDelete(DeleteBehavior.Cascade);

        });

        builder.Entity<Extra>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
        });


        builder.Entity<OrderExtra>(entity =>
        {
            // Composite primary key tanımlaması
            entity.HasKey(oe => new { oe.OrderId, oe.ExtraId });

            entity.Property(oe => oe.Quantity).IsRequired();

            // Order ile ilişkiyi kuruyoruz
            entity.HasOne(oe => oe.Order)
                  .WithMany(o => o.OrderExtras)
                  .HasForeignKey(oe => oe.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Extra ile ilişkiyi kuruyoruz
            entity.HasOne(oe => oe.Extra)
                  .WithMany()
                  .HasForeignKey(oe => oe.ExtraId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
