using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using GEAPP_API.Data;

var builder = WebApplication.CreateBuilder(args);

// Escuchar en todas las interfaces de red (no solo localhost)
builder.WebHost.UseUrls("http://0.0.0.0:9404", "https://localhost:9402");

// ── Base de datos GEAPP ────────────────────────────────────────────────────────
builder.Services.AddDbContext<GEAPPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GEAPP")));

// ── Base de datos ServempSys (segundo servidor) ────────────────────────────────
builder.Services.AddDbContext<ServempSysContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ServempSys")));

// ── Autenticación JWT (OWASP API2) ─────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key no configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.Zero
        };
        // No revelar detalles del error de autenticación
        options.Events = new JwtBearerEvents
        {
            OnChallenge = ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode = 401;
                ctx.Response.ContentType = "application/json";
                return ctx.Response.WriteAsync("{\"message\":\"No autorizado\"}");
            }
        };
    });

builder.Services.AddAuthorization();

// ── Rate Limiting (OWASP API4 / API6) ─────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    // Límite estricto en login para prevenir fuerza bruta
    options.AddFixedWindowLimiter("login", cfg =>
    {
        cfg.PermitLimit            = 5;
        cfg.Window                 = TimeSpan.FromMinutes(1);
        cfg.QueueProcessingOrder   = QueueProcessingOrder.OldestFirst;
        cfg.QueueLimit             = 0;
    });
    // Límite global por IP para endpoints normales
    options.AddFixedWindowLimiter("api", cfg =>
    {
        cfg.PermitLimit            = 100;
        cfg.Window                 = TimeSpan.FromMinutes(1);
        cfg.QueueProcessingOrder   = QueueProcessingOrder.OldestFirst;
        cfg.QueueLimit             = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ── CORS restrictivo (OWASP API8) ──────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfigured", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("X-Total-Count", "X-Page", "X-Page-Size"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger con soporte JWT ────────────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "GEAPP API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme      = "Bearer",
        BearerFormat = "JWT",
        In          = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa el token JWT obtenido en /api/Usuario/login"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Swagger solo en Development (OWASP API8) ───────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ── HTTPS + HSTS solo en producción (OWASP API8) ──────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

// ── Security Headers (OWASP API8) ─────────────────────────────────────────────
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"]  = "nosniff";
    headers["X-Frame-Options"]         = "DENY";
    headers["X-XSS-Protection"]        = "1; mode=block";
    headers["Referrer-Policy"]         = "strict-origin-when-cross-origin";
    headers["Permissions-Policy"]      = "camera=(), microphone=(), geolocation=()";
    headers["Content-Security-Policy"] = "default-src 'self'";
    // Quitar header que expone tecnología (OWASP API8)
    headers.Remove("Server");
    headers.Remove("X-Powered-By");
    await next();
});

// ── Manejo global de excepciones (no filtrar stack traces) ────────────────────
app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    ctx.Response.StatusCode  = 500;
    ctx.Response.ContentType = "application/json";

    var exFeature = ctx.Features
        .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

    // En Development mostramos el error real (incluyendo InnerException) para diagnóstico
    string msg;
    if (app.Environment.IsDevelopment() && exFeature?.Error != null)
    {
        var ex = exFeature.Error;
        // DbUpdateException oculta el detalle real en InnerException
        var detail = ex.InnerException?.Message ?? ex.Message;
        msg = detail.Replace("\"", "'").Replace("\r", " ").Replace("\n", " ");
    }
    else
    {
        msg = "Error interno del servidor";
    }

    await ctx.Response.WriteAsync($"{{\"message\":\"{msg}\"}}");
}));

// ── Pipeline ───────────────────────────────────────────────────────────────────
app.UseCors("AllowConfigured");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
