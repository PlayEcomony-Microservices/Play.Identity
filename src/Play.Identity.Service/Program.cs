using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Play.Common.Settings;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Settings;
using Play.Identity.Service.HostedServices;
using Play.Common.MassTransit;
using MassTransit;
using Play.Identity.Service.Exceptions;
using GreenPipes;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

var services = builder.Services;

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

var serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
var identityServerSettings = Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();

const string AllowedOriginSetting = "AllowedOrigin";

services.Configure<IdentitySettings>(Configuration.GetSection(nameof(IdentitySettings)))
        .AddDefaultIdentity<ApplicationUser>()
        .AddRoles<ApplicationRole>()
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (
            mongoDbSettings.ConnectionString,
            serviceSettings.ServiceName
        );

services.AddMassTransitWithMessageBroker(Configuration, retryConfigurator =>
{
    retryConfigurator.Interval(3, TimeSpan.FromSeconds(4));
    retryConfigurator.Ignore(typeof(UnknownUserException));
    retryConfigurator.Ignore(typeof(InsufficientFundsException));
});

services.AddIdentityServer(options =>
        {
            options.Events.RaiseSuccessEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseErrorEvents = true;
        })
        .AddAspNetIdentity<ApplicationUser>()
        .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
        .AddInMemoryApiResources(identityServerSettings.ApiResources)
        .AddInMemoryClients(identityServerSettings.Clients)
        .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
        .AddDeveloperSigningCredential();

services.AddLocalApiAuthentication();

services.AddControllers();

services.AddHostedService<IdentitySeedHostedService>();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Identity.Service", Version = "v1" });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Identity.Service v1"));


    app.UseCors(builder =>
    {
        builder.WithOrigins(Configuration[AllowedOriginSetting])
        .AllowAnyHeader()
        .AllowAnyHeader();
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.MapControllers();
app.MapRazorPages();

app.Run();