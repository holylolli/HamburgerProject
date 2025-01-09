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
    //Fluent api'de foreign keyler fazla tanımlandığı için bir düzeltilme yapıldı. TotalPrice ve Price hata verdiği için HasColumnType Eklendi. Migrationslar birkaç defa sıfırlanarak tekrar denendi.
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

            entity.Property(o => o.TotalPrice)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)"); // veya HasPrecision(18, 2)

            // ApplicationUser ile ilişki
            entity.HasOne(o => o.ApplicationUser)
                  .WithMany()
                  .HasForeignKey(o => o.ApplicationUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Menu ile ilişki
            entity.HasOne(o => o.Menu)
                  .WithMany(m => m.Orders)
                  .HasForeignKey(o => o.MenuId)
                  .OnDelete(DeleteBehavior.Restrict);

            // OrderExtras ile ilişki
            entity.HasMany(o => o.OrderExtras)
                  .WithOne(oe => oe.Order)
                  .HasForeignKey(oe => oe.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Extra>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Price)
                  .IsRequired()
                  .HasPrecision(18, 2); // 18 basamaklı, 2 ondalık kısmı olan bir decimal türü
        });


        builder.Entity<OrderExtra>(entity =>
        {
            entity.HasKey(oe => new { oe.OrderId, oe.ExtraId }); // Composite Key

            entity.HasOne(oe => oe.Order)
                  .WithMany(o => o.OrderExtras)
                  .HasForeignKey(oe => oe.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oe => oe.Extra)
                  .WithMany(e => e.OrderExtras)
                  .HasForeignKey(oe => oe.ExtraId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
