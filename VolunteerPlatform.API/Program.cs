using VolunteerPlatform.Data;
using Microsoft.EntityFrameworkCore;
using VolunteerPlatform.Service.Interfaces;
using VolunteerPlatform.Service.Concrete;
using VolunteerPlatform.API.Middlewares;
using VolunteerPlatform.API.Hubs;
using VolunteerPlatform.Data.Seeding;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVİS KAYITLARI (Build() öncesi) ---

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5174", "http://127.0.0.1:5173","http://127.0.0.1:5174", "http://localhost:5174","http://localhost:5175", "http://127.0.0.1:5175")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()); // SignalR ve Cookie desteği için
});

builder.Services.AddControllers();
builder.Services.AddSignalR(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injections SCD Type 2 mantığı GenericService içinde kurgulanmalı 
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISkillMatchingService, SkillMatchingService>();
builder.Services.AddScoped<IProjectDiscoveryService, ProjectDiscoveryService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMediaService, MediaService>();

var app = builder.Build();

// --- 2. MİDDLEWARE PIPELINE (Build() sonrası sıralama kritiktir) ---

// Global Hata Yönetimi her zaman en üstte
app.UseMiddleware<GlobalExceptionMiddleware>();

// Database Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await DbSeeder.SeedAsync(context); // Seeding hazırsa açabilirsin
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı seeding işlemi sırasında hata oluştu.");
    }
}

// Geliştirme ortamı araçları
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS Middleware - Routing ve Auth'dan önce gelmeli
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

// Auth yapısını kurduğunda buraya UseAuthentication ve UseAuthorization gelecek
app.UseAuthorization();

app.MapControllers();

// SignalR Hub Endpoint
app.MapHub<ChatHub>("/chathub"); 

app.Run();