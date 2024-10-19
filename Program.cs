using System.Reflection;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestApi;
using RestApi.Src.Config;
using RestApi.Src.Middlewares;
using RestApi.Src.Validations;

var builder = WebApplication.CreateBuilder(args);

var secret = new Secret(builder.Configuration);
var clientOne = new CorsOrigin { Name = "clientOne", Origin = "http://localhost:5173" };

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// cross-origin-request (cors service)
builder.Services.AddCors(opts =>
{
    opts.AddPolicy(
        name: clientOne.Name,
        builder =>
        {
            builder
                .WithOrigins(clientOne.Origin)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

builder.Services.AddControllers();

// register mediatr pipeline
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>)); // register custom behavior
});

// register all fluentvalidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// register jwt verify token service
builder
    .Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(secret.GetJwtSecretToken())
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
        x.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Request.Cookies.TryGetValue("access_token", out var accessToken);
                context.Token = accessToken;
                return Task.CompletedTask;
            },
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(clientOne.Name);

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalErrorHandling>();

app.MapControllers();

app.Run();
