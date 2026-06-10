using FlatLifeFrontend.Components;
using FlatLifeFrontend.Components.Pages;
using FlatLifeFrontend.Services;
using FlatLifeFrontend.Services.HouseholdTaskService;

var builder = WebApplication.CreateBuilder(args);

// <StaticComponent>
//     <InteractiveComponent RenderMode="RenderModeInteractiveServer" />
// </StaticComponent>

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient();

builder.Services.AddScoped<TodoService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<HouseholdTaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
