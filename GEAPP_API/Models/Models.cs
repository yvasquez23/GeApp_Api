using System.ComponentModel.DataAnnotations;

namespace GEAPP_API.Models;

public class Articulo
{
    public int Articulo_ID { get; set; }
    public int? Equivalente_ID { get; set; }

    [MaxLength(50)]
    public string? Articulo_CD { get; set; }

    [MaxLength(200)]
    public string? Articulo_Desc { get; set; }

    public int? Departamento_ID { get; set; }
    public int? Modelo_ID { get; set; }
    public int? Color_ID { get; set; }
    public int? Cuerpo_ID { get; set; }
    public int? Via_ID { get; set; }
    public int? Vidrio_ID { get; set; }
    public int? VidrioColor_ID { get; set; }
    public int? Calibre_ID { get; set; }
    public bool? Cierre { get; set; }
    public bool? Cerradura { get; set; }
    public bool? Reforzado { get; set; }
    public bool? Troquelado { get; set; }
}

public class Calibre
{
    public int Calibre_ID { get; set; }

    [MaxLength(100)]
    public string? Descripcion { get; set; }
}

public class Color
{
    public int Color_ID { get; set; }

    [MaxLength(100)]
    public string? Descripcion { get; set; }
}

public class Cotizacion
{
    public int Cotizacion_id { get; set; }
    public DateTime? Cotizacion_Fecha { get; set; }
    public int? Cliente_id { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El balance debe ser un valor positivo.")]
    public decimal? Cotizacion_Balance { get; set; }
}

public class Cotizacion_Det
{
    public int      Cotizacion_ID    { get; set; }
    public int      Cotizacion_DetID { get; set; }
    public decimal? Cotizacion_Qty    { get; set; }  // numeric en GEAPP
    public string?  Cotizacion_Precio { get; set; }  // varchar en GEAPP
}

public class Cuerpo
{
    public int Cuerpo_ID { get; set; }

    [MaxLength(100)]
    public string? Descripcion { get; set; }
}

public class Departamento
{
    public int Departamentos_id { get; set; }

    [MaxLength(100)]
    public string? Descripcion { get; set; }
}

public class Empresa
{
    public int Empersa_id { get; set; }

    [MaxLength(500)]
    public string? Info { get; set; }
}

public class Modelo
{
    public int Modelo_ID { get; set; }

    [MaxLength(150)]
    public string? Modelo_Desc { get; set; }
}

public class Monitor_Orden
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string? ordenproduccion_no { get; set; }
}

public class OPEstatus
{
    public int OPEstatus_id { get; set; }

    [MaxLength(100)]
    public string? Estatus_Desc { get; set; }
}

public class OrdenProduccion
{
    public int OrdenProduccion_id { get; set; }

    [MaxLength(50)]
    public string? OrdenProduccion_No { get; set; }

    [MaxLength(50)]
    public string? Hueco_No { get; set; }

    public int? Cliente_id { get; set; }
    public int? Articulo_ID { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Cantidad { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Ancho { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Largo { get; set; }

    public DateTime? OrdenProduccion_Fecha { get; set; }
    public int? OPEstatus_id { get; set; }
}

public class Usuario
{
    public int Usuario_id { get; set; }

    [Required]
    [MaxLength(100)]
    public string? usuario { get; set; }

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string? Correo { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "La contraseña debe tener mínimo 8 caracteres.")]
    [MaxLength(200)]
    public string? Contraseña { get; set; }

    public DateTime? Fecha_alta { get; set; }
    public bool? Estado { get; set; }
}

public class Usuario_Empresa
{
    public int Empresa_id { get; set; }
    public int Usuario_id { get; set; }

    [MaxLength(100)]
    public string? Usuario_pagina { get; set; }

    [MaxLength(200)]
    public string? Contraseña { get; set; }

    public int? Cliente_id { get; set; }
}

public class Via
{
    public int Via_ID { get; set; }

    [MaxLength(100)]
    public string? Descripcion { get; set; }
}

public class Vidrio
{
    public int Vidrio_ID { get; set; }

    [MaxLength(100)]
    public string? Descripcion { get; set; }
}

public class Vidrio_Color
{
    public int VidrioColor_ID { get; set; }

    [MaxLength(100)]
    public string? Descripcion { get; set; }
}
