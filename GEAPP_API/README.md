# GEAPP API

API REST en C# .NET 8 para la base de datos GEAPP conectada a SQL Server.

## Requisitos
- .NET 8 SDK → https://dotnet.microsoft.com/download
- SQL Server local con la base de datos GEAPP

## Configuración

1. Edita `appsettings.json` y reemplaza:
   - `TU_USUARIO` → tu usuario de SQL Server
   - `TU_CONTRASEÑA` → tu contraseña

   Si usas autenticación de Windows usa:
   ```
   "Server=localhost;Database=GEAPP;Trusted_Connection=True;TrustServerCertificate=True;"
   ```

## Instalación y ejecución

```bash
# Restaurar paquetes
dotnet restore

# Ejecutar
dotnet run
```

La API estará disponible en: http://localhost:5000
Documentación Swagger: http://localhost:5000/swagger

## Endpoints disponibles (CRUD completo)

| Recurso           | GET All              | GET by ID             | POST                  | PUT                   | DELETE                |
|-------------------|----------------------|-----------------------|-----------------------|-----------------------|-----------------------|
| Articulos         | GET /api/articulos   | GET /api/articulos/1  | POST /api/articulos   | PUT /api/articulos/1  | DELETE /api/articulos/1  |
| Calibre           | GET /api/calibre     | GET /api/calibre/1    | POST /api/calibre     | PUT /api/calibre/1    | DELETE /api/calibre/1    |
| Color             | GET /api/color       | GET /api/color/1      | POST /api/color       | PUT /api/color/1      | DELETE /api/color/1      |
| Cotizacion        | GET /api/cotizacion  | GET /api/cotizacion/1 | POST /api/cotizacion  | PUT /api/cotizacion/1 | DELETE /api/cotizacion/1 |
| CotizacionDet     | GET /api/cotizaciondet | GET /api/cotizaciondet/1/1 | POST /api/cotizaciondet | PUT /api/cotizaciondet/1/1 | DELETE /api/cotizaciondet/1/1 |
| Cuerpos           | GET /api/cuerpos     | GET /api/cuerpos/1    | POST /api/cuerpos     | PUT /api/cuerpos/1    | DELETE /api/cuerpos/1    |
| Departamentos     | GET /api/departamentos | GET /api/departamentos/1 | POST /api/departamentos | PUT /api/departamentos/1 | DELETE /api/departamentos/1 |
| Empresa           | GET /api/empresa     | GET /api/empresa/1    | POST /api/empresa     | PUT /api/empresa/1    | DELETE /api/empresa/1    |
| Modelo            | GET /api/modelo      | GET /api/modelo/1     | POST /api/modelo      | PUT /api/modelo/1     | DELETE /api/modelo/1     |
| MonitorOrden      | GET /api/monitororden | GET /api/monitororden/1 | POST /api/monitororden | PUT /api/monitororden/1 | DELETE /api/monitororden/1 |
| OPEstatus         | GET /api/opestatus   | GET /api/opestatus/1  | POST /api/opestatus   | PUT /api/opestatus/1  | DELETE /api/opestatus/1  |
| OrdenProduccion   | GET /api/ordenproduccion | GET /api/ordenproduccion/1 | POST /api/ordenproduccion | PUT /api/ordenproduccion/1 | DELETE /api/ordenproduccion/1 |
| Usuario           | GET /api/usuario     | GET /api/usuario/1    | POST /api/usuario     | PUT /api/usuario/1    | DELETE /api/usuario/1    |
| Vias              | GET /api/vias        | GET /api/vias/1       | POST /api/vias        | PUT /api/vias/1       | DELETE /api/vias/1       |
| Vidrio            | GET /api/vidrio      | GET /api/vidrio/1     | POST /api/vidrio      | PUT /api/vidrio/1     | DELETE /api/vidrio/1     |
| VidrioColor       | GET /api/vidriocolor | GET /api/vidriocolor/1 | POST /api/vidriocolor | PUT /api/vidriocolor/1 | DELETE /api/vidriocolor/1 |

## Login de usuarios

```
POST /api/usuario/login
Body: { "Correo": "user@email.com", "Contraseña": "123456" }
```

Las contraseñas se guardan con hash BCrypt de forma segura.
