using Microsoft.EntityFrameworkCore;
using GEAPP_API.Models;

namespace GEAPP_API.Data;

public class ServempSysContext : DbContext
{
    public ServempSysContext(DbContextOptions<ServempSysContext> options) : base(options) { }

    public DbSet<CotizacionExt>    Cotizacion    { get; set; }
    public DbSet<CotizacionDetExt> CotizacionDet { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cotizacion — PK simple con IDENTITY
        modelBuilder.Entity<CotizacionExt>(e =>
        {
            e.ToTable("Cotizacion");
            e.HasKey(c => c.Cotizacion_ID);
            e.Property(c => c.Cotizacion_ID).ValueGeneratedOnAdd();
            e.Property(c => c.Comentario).HasColumnType("text");
        });

        // CotizacionDet — PK compuesta, DetID es IDENTITY
        modelBuilder.Entity<CotizacionDetExt>(e =>
        {
            e.ToTable("CotizacionDet");
            e.HasKey(c => new { c.Cotizacion_ID, c.Cotizacion_DetID });
            e.Property(c => c.Cotizacion_DetID).ValueGeneratedOnAdd();
            e.Property(c => c.Cotizacion_ComentarioMemo).HasColumnType("nvarchar(max)");
        });
    }
}
