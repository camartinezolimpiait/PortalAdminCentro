using portalAdministrativoSISEC;
using portalAdministrativoSISEC.Util.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddValidatedOptions(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddConfiguredHttpClients(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseSession();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

public partial class Program
{
}
