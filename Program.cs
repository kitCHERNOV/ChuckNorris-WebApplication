using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure server service with dependency injection
builder.Services.AddScoped<ServerService>(sp =>
{
    var servers = new List<Server>();
    var newServer = new Server();
    return new ServerService(servers, newServer);
});

// Configure Active Directory Service
builder.Services.AddScoped<ActiveDirectoryService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var domainName = configuration["ActiveDirectory:DomainName"];
    var container = configuration["ActiveDirectory:Container"];
    return new ActiveDirectoryService(domainName, container);
});

var app = builder.Build();

// Configure middleware and request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
