using VolunteerPlatform.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(VolunteerPlatform.Service.Interfaces.IGenericService<>), typeof(VolunteerPlatform.Service.Concrete.GenericService<>));
builder.Services.AddScoped<VolunteerPlatform.Service.Interfaces.IApplicationService, VolunteerPlatform.Service.Concrete.ApplicationService>();
builder.Services.AddScoped<VolunteerPlatform.Service.Interfaces.IMessageService, VolunteerPlatform.Service.Concrete.MessageService>();
builder.Services.AddScoped<VolunteerPlatform.Service.Interfaces.IProjectService, VolunteerPlatform.Service.Concrete.ProjectService>();
//! later  builder.Services.AddScoped<VolunteerPlatform.Service.Interfaces.IReportService, VolunteerPlatform.Service.Concrete.ReportService>();
builder.Services.AddScoped<VolunteerPlatform.Service.Interfaces.ISkillService, VolunteerPlatform.Service.Concrete.SkillService>();
builder.Services.AddScoped<VolunteerPlatform.Service.Interfaces.IUserService, VolunteerPlatform.Service.Concrete.UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

