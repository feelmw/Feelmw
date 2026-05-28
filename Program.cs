using FeelmwLogistika.Blazor.Components;
using FeelmwLogistika.Blazor.Infrastructure.Documents;
using FeelmwLogistika.Blazor.Infrastructure.Excel;
using FeelmwLogistika.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<ILogistikaService, LogistikaService>();
builder.Services.AddScoped<IPlangintzaService, PlangintzaService>();
builder.Services.AddScoped<LogistikaWorkflowState>();
builder.Services.AddScoped<PlangintzaWorkflowState>();
builder.Services.AddScoped<IDocumentWorkflowService, DocumentWorkflowService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
builder.Services.AddScoped<ILogistikaDbService, LogistikaDbService>();
builder.Services.AddScoped<ILogistikaDataService, LogistikaDataService>();
builder.Services.AddScoped<IPlangintzaListakService, PlangintzaListakService>();
builder.Services.AddScoped<IExcelInfrastructureService, ExcelInfrastructureService>();
builder.Services.AddScoped<ILogistikaExcelService, LogistikaExcelService>();
builder.Services.AddScoped<IPlangintzaExcelService, PlangintzaExcelService>();
builder.Services.AddScoped<IDocumentTemplateService, DocumentTemplateService>();
builder.Services.AddScoped<ILogistikaDocumentService, LogistikaDocumentService>();
builder.Services.AddScoped<IPlangintzaDocumentService, PlangintzaDocumentService>();

await builder.Build().RunAsync();
