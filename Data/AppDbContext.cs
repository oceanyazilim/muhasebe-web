using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MuhasebeApp.Web.Models;

namespace MuhasebeApp.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cari> Cariler => Set<Cari>();
    public DbSet<Hareket> Hareketler => Set<Hareket>();
    public DbSet<OdemeNotu> OdemeNotlari => Set<OdemeNotu>();
    public DbSet<Fatura> Faturalar => Set<Fatura>();
    public DbSet<Dekont> Dekontlar => Set<Dekont>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Cari>().HasIndex(x => new { x.UserId, x.Kod });
        b.Entity<Cari>().HasIndex(x => x.UserId);

        b.Entity<Hareket>().HasIndex(x => new { x.UserId, x.CariId });
        b.Entity<Hareket>()
            .HasOne(h => h.Cari)
            .WithMany()
            .HasForeignKey(h => h.CariId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<OdemeNotu>().HasIndex(x => new { x.UserId, x.CariId });
        b.Entity<OdemeNotu>().HasIndex(x => new { x.UserId, x.Yon, x.Durum });
        b.Entity<OdemeNotu>()
            .HasOne(o => o.Cari)
            .WithMany()
            .HasForeignKey(o => o.CariId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Fatura>().HasIndex(x => new { x.UserId, x.CariId });
        b.Entity<Fatura>().HasIndex(x => new { x.UserId, x.FaturaNo });
        b.Entity<Fatura>()
            .HasOne(f => f.Cari)
            .WithMany()
            .HasForeignKey(f => f.CariId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Dekont>().HasIndex(x => x.UserId);

        foreach (var entity in b.Model.GetEntityTypes())
        {
            foreach (var prop in entity.GetProperties())
            {
                if (prop.ClrType == typeof(decimal) || prop.ClrType == typeof(decimal?))
                {
                    prop.SetColumnType("decimal(18,2)");
                }
            }
        }
    }
}
