using Microsoft.EntityFrameworkCore;
using GEAPP_API.Models;

namespace GEAPP_API.Data;

public class GEAPPContext : DbContext
{
    public GEAPPContext(DbContextOptions<GEAPPContext> options) : base(options) { }

    public DbSet<Articulo> Articulos { get; set; }
    public DbSet<Calibre> Calibre { get; set; }
    public DbSet<Color> Color { get; set; }
    public DbSet<Cotizacion> Cotizacion { get; set; }
    public DbSet<Cotizacion_Det> Cotizacion_Det { get; set; }
    public DbSet<Cuerpo> Cuerpos { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Empresa> Empresa { get; set; }
    public DbSet<Modelo> Modelo { get; set; }
    public DbSet<Monitor_Orden> Monitor_Orden { get; set; }
    public DbSet<OPEstatus> OPEstatus { get; set; }
    public DbSet<OrdenProduccion> OrdenProduccion { get; set; }
    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<Usuario_Empresa> Usuario_Empresa { get; set; }
    public DbSet<Via> Vias { get; set; }
    public DbSet<Vidrio> Vidrio { get; set; }
    public DbSet<Vidrio_Color> Vidrio_Color { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Articulo>().HasKey(a => a.Articulo_ID);
        modelBuilder.Entity<Calibre>().HasKey(c => c.Calibre_ID);
        modelBuilder.Entity<Color>().HasKey(c => c.Color_ID);
        modelBuilder.Entity<Cotizacion>(e =>
        {
            e.HasKey(c => c.Cotizacion_id);
            e.Property(c => c.Cotizacion_id).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<Cotizacion_Det>(e =>
        {
            e.HasKey(c => new { c.Cotizacion_ID, c.Cotizacion_DetID });
            // Cotizacion_DetID es IDENTITY en SQL Server — la BD asigna el valor
            e.Property(c => c.Cotizacion_DetID).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<Cuerpo>().HasKey(c => c.Cuerpo_ID);
        modelBuilder.Entity<Departamento>().HasKey(d => d.Departamentos_id);
        modelBuilder.Entity<Empresa>().HasKey(e => e.Empersa_id);
        modelBuilder.Entity<Modelo>().HasKey(m => m.Modelo_ID);
        modelBuilder.Entity<Monitor_Orden>().HasKey(m => m.Id);
        modelBuilder.Entity<OPEstatus>().HasKey(o => o.OPEstatus_id);
        modelBuilder.Entity<OrdenProduccion>().HasKey(o => o.OrdenProduccion_id);
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(u => u.Usuario_id);
            // Usuario_id es IDENTITY en SQL Server — la BD lo asigna
            e.Property(u => u.Usuario_id).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<Usuario_Empresa>().HasKey(u => new { u.Empresa_id, u.Usuario_id });
        modelBuilder.Entity<Via>().HasKey(v => v.Via_ID);
        modelBuilder.Entity<Vidrio>().HasKey(v => v.Vidrio_ID);
        modelBuilder.Entity<Vidrio_Color>().HasKey(v => v.VidrioColor_ID);
    }
}
