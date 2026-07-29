using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GEAPP_API.Data;
using GEAPP_API.Models;

namespace GEAPP_API.Controllers;

// ═══════════════════════════════════════════════════════════════════════════════
// ARTÍCULOS
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArticulosController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public ArticulosController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Articulos.CountAsync();
        var items = await _ctx.Articulos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        Response.Headers["X-Page"]        = page.ToString();
        Response.Headers["X-Page-Size"]   = pageSize.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _ctx.Articulos.FindAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Articulo item)
    {
        _ctx.Articulos.Add(item);
        await _ctx.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Articulo_ID }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Articulo item)
    {
        if (id != item.Articulo_ID) return BadRequest();
        _ctx.Entry(item).State = EntityState.Modified;
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _ctx.Articulos.FindAsync(id);
        if (item == null) return NotFound();
        _ctx.Articulos.Remove(item);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CALIBRE
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalibreController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public CalibreController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Calibre.CountAsync();
        var items = await _ctx.Calibre.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Calibre.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Calibre item) { _ctx.Calibre.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Calibre_ID }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Calibre item) { if (id != item.Calibre_ID) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Calibre.FindAsync(id); if (item == null) return NotFound(); _ctx.Calibre.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// COLOR
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ColorController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public ColorController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Color.CountAsync();
        var items = await _ctx.Color.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Color.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Color item) { _ctx.Color.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Color_ID }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Color item) { if (id != item.Color_ID) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Color.FindAsync(id); if (item == null) return NotFound(); _ctx.Color.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// COTIZACIÓN
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>DTO para crear cotización completa (header + detalles) en ambas BDs</summary>
public class CotizacionCompletaRequest
{
    public DateTime? Cotizacion_Fecha    { get; set; }
    public int?      Cliente_ID          { get; set; }
    public string?   Cotizacion_Cliente  { get; set; }
    public decimal?  Cotizacion_Balance  { get; set; }
    public string?   Comentario          { get; set; }
    public List<DetalleRequest> Detalles { get; set; } = new();
}

public class DetalleRequest
{
    public int?     Articulo_ID          { get; set; }
    public decimal? Cotizacion_Qty       { get; set; }
    public decimal? Cotizacion_Precio    { get; set; }
    public string?  Comentario           { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CotizacionController : ControllerBase
{
    private readonly GEAPPContext      _ctx;
    private readonly ServempSysContext _ext;

    public CotizacionController(GEAPPContext ctx, ServempSysContext ext)
    {
        _ctx = ctx;
        _ext = ext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int  page       = 1,
        [FromQuery] int  pageSize   = 50,
        [FromQuery] int? clienteId  = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = _ctx.Cotizacion.AsQueryable();
        if (clienteId.HasValue)
            query = query.Where(c => c.Cliente_id == clienteId.Value);
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.Cotizacion_Fecha)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Cotizacion.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Cotizacion item) { _ctx.Cotizacion.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Cotizacion_id }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Cotizacion item) { if (id != item.Cotizacion_id) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Cotizacion.FindAsync(id); if (item == null) return NotFound(); _ctx.Cotizacion.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }

    // ── POST /api/Cotizacion/completa ──────────────────────────────────────────
    /// <summary>
    /// Crea la cotización (header + detalles) en GEAPP y en ServempSys
    /// simultáneamente. Si ServempSys falla, se registra el error pero la
    /// operación en GEAPP se mantiene.
    /// </summary>
    [HttpPost("completa")]
    public async Task<IActionResult> CreateCompleta([FromBody] CotizacionCompletaRequest req)
    {
        // ── 1. Insertar en GEAPP ───────────────────────────────────────────────
        var cotGeapp = new Cotizacion
        {
            Cotizacion_Fecha   = req.Cotizacion_Fecha,
            Cliente_id         = req.Cliente_ID,
            Cotizacion_Balance = req.Cotizacion_Balance,
        };
        _ctx.Cotizacion.Add(cotGeapp);
        await _ctx.SaveChangesAsync();

        // Detalles en GEAPP
        for (int i = 0; i < req.Detalles.Count; i++)
        {
            var d = req.Detalles[i];
            _ctx.Cotizacion_Det.Add(new Cotizacion_Det
            {
                Cotizacion_ID     = cotGeapp.Cotizacion_id,
                // Cotizacion_DetID: IDENTITY, la BD lo genera automáticamente
                Cotizacion_Qty    = d.Cotizacion_Qty,               // numeric en GEAPP
                Cotizacion_Precio = d.Cotizacion_Precio?.ToString(), // varchar en GEAPP
            });
        }
        await _ctx.SaveChangesAsync();

        // ── 2. Insertar en ServempSys ──────────────────────────────────────────
        string? extError = null;
        int?    extId    = null;
        try
        {
            var cotExt = new CotizacionExt
            {
                Cotizacion_Fecha   = req.Cotizacion_Fecha,
                Cliente_ID         = req.Cliente_ID,
                Cotizacion_Cliente = req.Cotizacion_Cliente,
                Cotizacion_Balance = req.Cotizacion_Balance,
                SubTotal           = req.Cotizacion_Balance,
                Comentario         = req.Comentario,
                // Guardamos el ID de GEAPP para poder correlacionar después
                Cotizacion_Datos   = cotGeapp.Cotizacion_id.ToString(),
            };
            _ext.Cotizacion.Add(cotExt);
            await _ext.SaveChangesAsync();
            extId = cotExt.Cotizacion_ID;

            // Detalles en ServempSys
            foreach (var d in req.Detalles)
            {
                _ext.CotizacionDet.Add(new CotizacionDetExt
                {
                    Cotizacion_ID        = cotExt.Cotizacion_ID,
                    Cotizacion_Qty       = (float?)d.Cotizacion_Qty,
                    Cotizacion_Precio    = d.Cotizacion_Precio,
                    Articulo_ID          = d.Articulo_ID,
                    Cotizacion_Comentario = d.Comentario,
                });
            }
            await _ext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // ServempSys falló, pero GEAPP ya está guardado
            extError = ex.Message;
        }

        return Ok(new
        {
            geapp_id   = cotGeapp.Cotizacion_id,
            ext_id     = extId,
            ext_error  = extError,
            detalles   = req.Detalles.Count,
        });
    }

    // ── GET /api/Cotizacion/{id}/detalles ──────────────────────────────────────
    /// <summary>
    /// Devuelve detalles con info de artículo usando ServempSys como fuente
    /// (tiene Articulo_ID, Qty, Precio bien tipados). Los nombres de artículo
    /// se obtienen de la tabla Articulos de GEAPP.
    /// Cotizacion_Datos actúa como enlace entre ambos sistemas.
    /// </summary>
    [HttpGet("{id:int}/detalles")]
    public async Task<IActionResult> GetDetallesConArticulo(int id)
    {
        var geappIdStr = id.ToString();
        try
        {
            // ── 1. Localizar cotización en ServempSys por el enlace guardado ──
            var extCot = await _ext.Cotizacion
                .FirstOrDefaultAsync(c => c.Cotizacion_Datos == geappIdStr);

            if (extCot == null)
                return Ok(Array.Empty<object>());

            // ── 2. Detalles desde ServempSys (tipos correctos + Articulo_ID) ──
            var extDets = await _ext.CotizacionDet
                .Where(d => d.Cotizacion_ID == extCot.Cotizacion_ID)
                .OrderBy(d => d.Cotizacion_DetID)
                .ToListAsync();

            // ── 3. Nombres de artículos desde GEAPP ───────────────────────────
            // Consulta individual por ID para evitar OPENJSON (incompatible
            // con SQL Server < 2016 / compatibilidad < 130)
            var artIds = extDets
                .Where(d => d.Articulo_ID != null)
                .Select(d => d.Articulo_ID!.Value)
                .Distinct().ToList();

            var artMap = new Dictionary<int, Articulo>();
            foreach (var artId in artIds)
            {
                var art = await _ctx.Articulos
                    .FirstOrDefaultAsync(a => a.Articulo_ID == artId);
                if (art != null) artMap[artId] = art;
            }

            // ── 4. Combinar ────────────────────────────────────────────────────
            var result = extDets.Select(d =>
            {
                artMap.TryGetValue(d.Articulo_ID ?? 0, out var art);
                return new
                {
                    detId        = d.Cotizacion_DetID,
                    qty          = (double?)d.Cotizacion_Qty,
                    precio       = (double?)d.Cotizacion_Precio,
                    articuloId   = d.Articulo_ID,
                    articuloCd   = art?.Articulo_CD,
                    articuloDesc = art?.Articulo_Desc,
                };
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // ── PUT /api/Cotizacion/{id}/completa ──────────────────────────────────────
    /// <summary>
    /// Actualiza cabecera + detalles de una cotización existente en GEAPP y
    /// ServempSys. Los detalles anteriores en ServempSys se eliminan y se
    /// reemplazan por los nuevos.
    /// </summary>
    [HttpPut("{id:int}/completa")]
    public async Task<IActionResult> UpdateCompleta(
        int id, [FromBody] CotizacionCompletaRequest req)
    {
        try
        {
            // ── 1. Actualizar cabecera en GEAPP ────────────────────────────────
            var cotGeapp = await _ctx.Cotizacion.FindAsync(id);
            if (cotGeapp == null)
                return NotFound(new { message = $"Cotización {id} no encontrada" });

            cotGeapp.Cotizacion_Fecha   = req.Cotizacion_Fecha;
            cotGeapp.Cotizacion_Balance = req.Cotizacion_Balance;
            await _ctx.SaveChangesAsync();

            // ── 2. Localizar cotización en ServempSys ──────────────────────────
            var geappIdStr = id.ToString();
            var extCot = await _ext.Cotizacion
                .FirstOrDefaultAsync(c => c.Cotizacion_Datos == geappIdStr);

            string? extError = null;

            if (extCot != null)
            {
                try
                {
                    // ── 3. Eliminar detalles antiguos en ServempSys ────────────
                    var oldDets = await _ext.CotizacionDet
                        .Where(d => d.Cotizacion_ID == extCot.Cotizacion_ID)
                        .ToListAsync();
                    _ext.CotizacionDet.RemoveRange(oldDets);
                    await _ext.SaveChangesAsync();

                    // ── 4. Insertar nuevos detalles en ServempSys ──────────────
                    foreach (var d in req.Detalles)
                    {
                        _ext.CotizacionDet.Add(new CotizacionDetExt
                        {
                            Cotizacion_ID      = extCot.Cotizacion_ID,
                            Articulo_ID        = d.Articulo_ID,
                            Cotizacion_Qty     = (float?)d.Cotizacion_Qty,
                            Cotizacion_Precio  = d.Cotizacion_Precio,
                            Cotizacion_Comentario = d.Comentario,
                        });
                    }

                    // ── 5. Actualizar balance en ServempSys ────────────────────
                    extCot.Cotizacion_Fecha   = req.Cotizacion_Fecha;
                    extCot.Cotizacion_Balance = req.Cotizacion_Balance;
                    extCot.SubTotal           = req.Cotizacion_Balance;

                    await _ext.SaveChangesAsync();
                }
                catch (Exception exExt)
                {
                    extError = exExt.Message;
                }
            }

            return Ok(new
            {
                geappId    = id,
                extId      = extCot?.Cotizacion_ID,
                extWarning = extError
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// COTIZACIÓN DETALLE
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CotizacionDetController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public CotizacionDetController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] int? cotizacionId = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 200);
        var query = _ctx.Cotizacion_Det.AsQueryable();
        if (cotizacionId.HasValue)
            query = query.Where(d => d.Cotizacion_ID == cotizacionId.Value);
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{cotizacionId:int}/{detId:int}")]
    public async Task<IActionResult> Get(int cotizacionId, int detId) { var item = await _ctx.Cotizacion_Det.FindAsync(cotizacionId, detId); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Cotizacion_Det item) { _ctx.Cotizacion_Det.Add(item); await _ctx.SaveChangesAsync(); return Ok(item); }

    [HttpPut("{cotizacionId:int}/{detId:int}")]
    public async Task<IActionResult> Update(int cotizacionId, int detId, [FromBody] Cotizacion_Det item) { if (cotizacionId != item.Cotizacion_ID || detId != item.Cotizacion_DetID) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{cotizacionId:int}/{detId:int}")]
    public async Task<IActionResult> Delete(int cotizacionId, int detId) { var item = await _ctx.Cotizacion_Det.FindAsync(cotizacionId, detId); if (item == null) return NotFound(); _ctx.Cotizacion_Det.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CUERPOS
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CuerposController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public CuerposController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Cuerpos.CountAsync();
        var items = await _ctx.Cuerpos.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Cuerpos.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Cuerpo item) { _ctx.Cuerpos.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Cuerpo_ID }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Cuerpo item) { if (id != item.Cuerpo_ID) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Cuerpos.FindAsync(id); if (item == null) return NotFound(); _ctx.Cuerpos.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// DEPARTAMENTOS
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartamentosController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public DepartamentosController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Departamentos.CountAsync();
        var items = await _ctx.Departamentos.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Departamentos.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Departamento item) { _ctx.Departamentos.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Departamentos_id }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Departamento item) { if (id != item.Departamentos_id) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Departamentos.FindAsync(id); if (item == null) return NotFound(); _ctx.Departamentos.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// EMPRESA
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpresaController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public EmpresaController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Empresa.CountAsync();
        var items = await _ctx.Empresa.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Empresa.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Empresa item) { _ctx.Empresa.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Empersa_id }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Empresa item) { if (id != item.Empersa_id) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Empresa.FindAsync(id); if (item == null) return NotFound(); _ctx.Empresa.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// MODELO
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ModeloController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public ModeloController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Modelo.CountAsync();
        var items = await _ctx.Modelo.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Modelo.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Modelo item) { _ctx.Modelo.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Modelo_ID }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Modelo item) { if (id != item.Modelo_ID) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Modelo.FindAsync(id); if (item == null) return NotFound(); _ctx.Modelo.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// MONITOR ORDEN
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MonitorOrdenController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public MonitorOrdenController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Monitor_Orden.CountAsync();
        var items = await _ctx.Monitor_Orden.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Monitor_Orden.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Monitor_Orden item) { _ctx.Monitor_Orden.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Id }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Monitor_Orden item) { if (id != item.Id) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Monitor_Orden.FindAsync(id); if (item == null) return NotFound(); _ctx.Monitor_Orden.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// OP ESTATUS
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OPEstatusController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public OPEstatusController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.OPEstatus.CountAsync();
        var items = await _ctx.OPEstatus.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.OPEstatus.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OPEstatus item) { _ctx.OPEstatus.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.OPEstatus_id }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] OPEstatus item) { if (id != item.OPEstatus_id) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.OPEstatus.FindAsync(id); if (item == null) return NotFound(); _ctx.OPEstatus.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// ORDEN PRODUCCIÓN
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenProduccionController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public OrdenProduccionController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.OrdenProduccion.CountAsync();
        var items = await _ctx.OrdenProduccion.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.OrdenProduccion.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrdenProduccion item) { _ctx.OrdenProduccion.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.OrdenProduccion_id }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] OrdenProduccion item) { if (id != item.OrdenProduccion_id) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.OrdenProduccion.FindAsync(id); if (item == null) return NotFound(); _ctx.OrdenProduccion.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// USUARIO
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    private readonly IConfiguration _config;

    public UsuarioController(GEAPPContext ctx, IConfiguration config)
    {
        _ctx    = ctx;
        _config = config;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Usuario.CountAsync();
        var items = await _ctx.Usuario
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            // Nunca exponer contraseñas (OWASP API3)
            .Select(u => new { u.Usuario_id, u.usuario, u.Correo, u.Fecha_alta, u.Estado })
            .ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _ctx.Usuario.FindAsync(id);
        return item == null
            ? NotFound()
            : Ok(new { item.Usuario_id, item.usuario, item.Correo, item.Fecha_alta, item.Estado });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Usuario item)
    {
        item.Contraseña = BCrypt.Net.BCrypt.HashPassword(item.Contraseña);
        _ctx.Usuario.Add(item);
        await _ctx.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Usuario_id },
            new { item.Usuario_id, item.usuario, item.Correo });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Usuario item)
    {
        if (id != item.Usuario_id) return BadRequest();
        var existing = await _ctx.Usuario.FindAsync(id);
        if (existing == null) return NotFound();
        existing.usuario = item.usuario;
        existing.Correo  = item.Correo;
        existing.Estado  = item.Estado;
        if (!string.IsNullOrEmpty(item.Contraseña))
            existing.Contraseña = BCrypt.Net.BCrypt.HashPassword(item.Contraseña);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _ctx.Usuario.FindAsync(id);
        if (item == null) return NotFound();
        _ctx.Usuario.Remove(item);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // ── SCHEMA DIAGNÓSTICO: tamaños reales de columnas (solo Development) ────────
    [HttpGet("schema")]
    [AllowAnonymous]
    public async Task<IActionResult> Schema()
    {
        var cols = await _ctx.Database
            .SqlQueryRaw<ColumnInfo>(
                "SELECT COLUMN_NAME as ColumnName, DATA_TYPE as DataType, " +
                "ISNULL(CHARACTER_MAXIMUM_LENGTH, -1) as MaxLength " +
                "FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE TABLE_NAME = 'Usuario'")
            .ToListAsync();
        return Ok(cols);
    }

    // ── REGISTRO: público ─────────────────────────────────────────────────────
    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<IActionResult> Registro([FromBody] RegistroRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Usuario) ||
            string.IsNullOrWhiteSpace(req.Correo)  ||
            string.IsNullOrWhiteSpace(req.Contraseña))
            return BadRequest(new { message = "Todos los campos son obligatorios" });

        // usuario: varchar(11) en BD
        if (req.Usuario.Length > 11)
            return BadRequest(new { message = "El nombre de usuario no puede tener más de 11 caracteres" });

        // Correo: varchar(50) en BD
        if (req.Correo.Length > 50)
            return BadRequest(new { message = "El correo no puede tener más de 50 caracteres" });

        if (await _ctx.Usuario.AnyAsync(u => u.Correo == req.Correo))
            return BadRequest(new { message = "El correo ya está registrado" });

        var user = new Usuario
        {
            usuario    = req.Usuario,
            Correo     = req.Correo,
            Contraseña = BCrypt.Net.BCrypt.HashPassword(req.Contraseña),
            Fecha_alta = DateTime.Now,
            Estado     = true,
        };
        _ctx.Usuario.Add(user);

        try
        {
            await _ctx.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            return StatusCode(500, new { message = $"Error al guardar: {detail}" });
        }

        var token = GenerateJwtToken(user);
        return Ok(new
        {
            user.Usuario_id,
            usuario   = user.usuario,
            user.Correo,
            token,
            expiresIn = int.Parse(_config["Jwt:ExpirationMinutes"] ?? "60") * 60
        });
    }

    // ── LOGIN: público + rate limiting anti fuerza bruta ───────────────────────
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        // Mensaje genérico para no revelar si el correo existe (OWASP API2)
        const string invalidMsg = "Credenciales inválidas";

        // Buscar por correo sin filtrar Estado, para no bloquear usuarios con Estado null
        var user = await _ctx.Usuario
            .FirstOrDefaultAsync(u => u.Correo == req.Correo);

        bool passwordOk = false;
        if (user != null)
        {
            try
            {
                // Contraseña hasheada con BCrypt
                passwordOk = BCrypt.Net.BCrypt.Verify(req.Contraseña, user.Contraseña);
            }
            catch
            {
                // La contraseña en BD es texto plano (datos legados): comparar directo
                passwordOk = user.Contraseña == req.Contraseña;
            }
        }

        if (!passwordOk)
            return Unauthorized(new { message = invalidMsg });

        var token = GenerateJwtToken(user);
        return Ok(new
        {
            user.Usuario_id,
            user.usuario,
            user.Correo,
            token,
            expiresIn = int.Parse(_config["Jwt:ExpirationMinutes"] ?? "60") * 60
        });
    }

    private string GenerateJwtToken(Usuario user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Usuario_id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Correo ?? ""),
            new Claim(JwtRegisteredClaimNames.Name,  user.usuario ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:            _config["Jwt:Issuer"],
            audience:          _config["Jwt:Audience"],
            claims:            claims,
            expires:           DateTime.UtcNow.AddMinutes(
                                   double.Parse(_config["Jwt:ExpirationMinutes"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Correo, string Contraseña);
public record RegistroRequest(string Usuario, string Correo, string Contraseña);

// DTO para leer esquema de columnas via SqlQueryRaw
public class ColumnInfo
{
    public string ColumnName { get; set; } = "";
    public string DataType   { get; set; } = "";
    public int    MaxLength  { get; set; }
}

// ═══════════════════════════════════════════════════════════════════════════════
// VÍAS
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ViasController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public ViasController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Vias.CountAsync();
        var items = await _ctx.Vias.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Vias.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Via item) { _ctx.Vias.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Via_ID }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Via item) { if (id != item.Via_ID) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Vias.FindAsync(id); if (item == null) return NotFound(); _ctx.Vias.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// VIDRIO
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VidrioController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public VidrioController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Vidrio.CountAsync();
        var items = await _ctx.Vidrio.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Vidrio.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Vidrio item) { _ctx.Vidrio.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.Vidrio_ID }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Vidrio item) { if (id != item.Vidrio_ID) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Vidrio.FindAsync(id); if (item == null) return NotFound(); _ctx.Vidrio.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// VIDRIO COLOR
// ═══════════════════════════════════════════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VidrioColorController : ControllerBase
{
    private readonly GEAPPContext _ctx;
    public VidrioColorController(GEAPPContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _ctx.Vidrio_Color.CountAsync();
        var items = await _ctx.Vidrio_Color.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id) { var item = await _ctx.Vidrio_Color.FindAsync(id); return item == null ? NotFound() : Ok(item); }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Vidrio_Color item) { _ctx.Vidrio_Color.Add(item); await _ctx.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = item.VidrioColor_ID }, item); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Vidrio_Color item) { if (id != item.VidrioColor_ID) return BadRequest(); _ctx.Entry(item).State = EntityState.Modified; await _ctx.SaveChangesAsync(); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { var item = await _ctx.Vidrio_Color.FindAsync(id); if (item == null) return NotFound(); _ctx.Vidrio_Color.Remove(item); await _ctx.SaveChangesAsync(); return NoContent(); }
}
