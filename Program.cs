using System.Text.Json;
using MediHub.Application.Interfaces;
using MediHub.Application.Services;
using MediHub.Infrastructure.Data;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// Configure Functions Web Application
builder.ConfigureFunctionsWebApplication();

// Application Insights
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Configuration
builder.Configuration
       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();


builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.PropertyNameCaseInsensitive = true;
});

builder.UseMiddleware<AuthAndCorsMiddleware>();



builder.Services.AddSingleton<SqlConnectionFactory>();


builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();

builder.Services.AddScoped<IFacilityRepository, FacilityRepository>();
builder.Services.AddScoped<IFacilityService, FacilityService>();

builder.Services.AddScoped<IInstanceRepository, InstanceRepository>();
builder.Services.AddScoped<IInstanceService, InstanceService>();

// builder.Services.AddScoped<IScheduleService, ScheduleService>();

builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ITemplateService, TemplateService>();

builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();

builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();

builder.Services.AddScoped<ISubspecialtyRepository, SubspecialtyRepository>();
builder.Services.AddScoped<ISubspecialtyService, SubspecialtyService>();

builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetService, AssetService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<ISessionOverrideRepository, SessionOverrideRepository>();
builder.Services.AddScoped<ISessionOverrideService, SessionOverrideService>();

builder.Services.AddScoped<ISurgeonTypeRepository, SurgeonTypeRepository>();
builder.Services.AddScoped<ISurgeonTypeService, SurgeonTypeService>();

builder.Services.AddScoped<IAnaestheticTypeRepository, AnaestheticTypeRepository>();
builder.Services.AddScoped<IAnaestheticTypeService, AnaestheticTypeService>();

builder.Build().Run();
