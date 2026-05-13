using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GEAPP_API.Models;

/// <summary>Cabecera de cotización en ServempSys</summary>
public class CotizacionExt
{
    public int Cotizacion_ID { get; set; }

    [MaxLength(20)]
    public string? Cotizacion_No { get; set; }

    public DateTime? Cotizacion_Fecha { get; set; }

    public int? CondPago_ID { get; set; }
    public int? Cliente_ID { get; set; }
    public int? Vendedor_ID { get; set; }

    [Column(TypeName = "money")]
    public decimal? Cotizacion_Descto { get; set; }

    [Column(TypeName = "money")]
    public decimal? Cotizacion_ITBIS { get; set; }

    [Column(TypeName = "money")]
    public decimal? Cotizacion_Balance { get; set; }

    [MaxLength(50)]
    public string? Cotizacion_Cliente { get; set; }

    [MaxLength(150)]
    public string? Cotizacion_Datos { get; set; }

    public int? Factura_ID { get; set; }
    public int? OrdenProduccion_ID { get; set; }
    public int? Almacen_ID { get; set; }

    [Column(TypeName = "money")]
    public decimal? SubTotal { get; set; }

    public string? Comentario { get; set; }

    public int? OrdenProdMatarial_ID { get; set; }
    public int? FacturaPre_ID { get; set; }

    [MaxLength(20)]
    public string? Cotizacion_Doc { get; set; }
}

/// <summary>Detalle de cotización en ServempSys</summary>
public class CotizacionDetExt
{
    public int Cotizacion_ID { get; set; }
    public int Cotizacion_DetID { get; set; }

    public int? Articulo_ID { get; set; }

    public float? Cotizacion_Qty { get; set; }

    [Column(TypeName = "money")]
    public decimal? Cotizacion_Precio { get; set; }

    [Column(TypeName = "money")]
    public decimal? Cotizacion_ITBIS { get; set; }

    [MaxLength(250)]
    public string? Cotizacion_Comentario { get; set; }

    public string? Cotizacion_ComentarioMemo { get; set; }

    public int? Unidad_ID { get; set; }
    public int? OrdenProduccion_ID { get; set; }
    public int? CotizacionDet_No { get; set; }

    public float? OPCant { get; set; }
    public float? OPAncho { get; set; }
    public float? OPLargo { get; set; }
    public float? OPAncho2 { get; set; }
    public float? OPFondo { get; set; }
}
