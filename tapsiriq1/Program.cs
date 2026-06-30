using Application;
using Application.Mappings;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Repositories; // OdataEntityInitializator üçün lazımi namespace-i əlavə edin
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization; // ReferenceHandler üçün lazımdır
using Microsoft.AspNetCore.Mvc; // JsonOptions üçün lazımdır
using tapsiriq1.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// --- 1. JWT AYARLARINI APPSMALL-DAN OXUYURUQ ---
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

// --- 2. HTTPCONTEXTACCESSOR QEYDİYYATI ---
builder.Services.AddHttpContextAccessor();


// --- 3. CONTROLLERS VƏ SİZİN ODATA / JSON AYARLARINIZ ---
builder.Services.AddControllers()
    .AddJsonOptions(options => ConfigureJson(options)) // Nöqtəli vergül burdan silindi!
    .AddOData(options =>
    {
        options.Select().Filter().OrderBy().Expand().Count(); // Ehtiyac olan funksiyaları aktiv edin
    });



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tapsiriq 1 API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Zəhmət olmasa tokeni daxil edin. Məsələn: Bearer eyJhbGciOi...",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// --- 4. AUTHENTICATION VƏ AUTHORIZATION QEYDİYYATLARI ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

// --- 5. DB_CONTEXT VƏ LAYİHƏ SERVİSLƏRİ ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

// --- 6. MIDDLEWARE-LƏR ---
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// OData marşrutlandırmasının ($odata) aktivləşməsi üçün bu mütləq bura əlavə olunmalıdır!
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --- 7. STATİK ROLLARIN BAZAYA YAZILMASI (SEED DATA) ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.SeedRoles(dbContext);
}

app.Run();

// --- SİZİN MƏRKƏZİ JSON AYAR METODUNUZ ---
void ConfigureJson(JsonOptions op)
{
    op.JsonSerializerOptions.PropertyNameCaseInsensitive = false; // Strict Case-Sensitive
    op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Dövr əngəlləmə
}