using Amazon.S3;
using EdsonRodrigoDev.Integracoes.Tecnologias.Application.Services;
using EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Configuracoes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon;
using Amazon.Runtime;
using EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Interfaces;
using EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProdutoAppServices>();
builder.Services.AddMassTransit(bus =>
{
    bus.UsingRabbitMq((ctx, busconfig) =>
    {
        
        busconfig.Host(builder.Configuration.GetConnectionString("RabbitMq"), "/", h => {
            h.Username(builder.Configuration.GetSection("RabbitMqConfig:User").Value);
            h.Password(builder.Configuration.GetSection("RabbitMqConfig:Password").Value);
        });

        busconfig.ConfigureEndpoints(ctx);
    });
});

builder.Services.Configure <AwsAppSetingsConfiguration>(builder.Configuration.GetSection("AwsAppSetingsConfiguration"));
string? queueName =
    builder.Configuration["MessageConfigurations:AwsConfigurations:QueueName"];
string? accessKey =
    builder.Configuration["MessageConfigurations:AwsConfigurations:AccessKey"];
string? secretKey =
    builder.Configuration["MessageConfigurations:AwsConfigurations:SecretKey"];

builder.Services.AddScoped<IAmazonS3>(sc =>
{
    var configuration = sc.GetRequiredService<IConfiguration>();
    var ServiceURLBucketS3 = configuration.GetSection("AwsAppSetingsConfiguration:ServiceURLBucketS3").Value;
    var AwsAccessKey = configuration.GetSection("AwsAppSetingsConfiguration:AwsAccessKey").Value;
    var AwsSecretKey = configuration.GetSection("AwsAppSetingsConfiguration:AwsSecretKey").Value;

    if (ServiceURLBucketS3 is null)
    {
        return new AmazonS3Client();
    }
    else
    {
        return new AmazonS3Client(AwsAccessKey, AwsSecretKey, new AmazonS3Config()
        {
            ForcePathStyle = true,
            ServiceURL = ServiceURLBucketS3,

        });
    }
});
builder.Services.AddScoped<IHandleAwsBucketS3Services, HandleAwsBucketS3Services>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
