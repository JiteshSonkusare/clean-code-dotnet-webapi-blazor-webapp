using MudBlazorClient.Extensions;
using Client.Infrastructure.Extensions;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

#region "Add services to the container"

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
builder.Services.AddAzureAuthDependencies(builder.Configuration)
                .RegiterDI()
				.AddMudBlazorServices()
				.AddClientInfrastrctureDependencies(builder.Configuration)
                .AddServerSideBlazor();

var app = builder.Build();

#endregion

#region "Configure the HTTP request pipeline"

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();

#endregion