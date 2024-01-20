using CurrencyApp.Services;
using FluentAssertions.Common;

namespace CurrencyApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();
       
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    builder => builder.WithOrigins("http://localhost:3000") 
                                        .AllowAnyHeader()
                                        .AllowAnyMethod());
            });

            var configuration = builder.Configuration;
            builder.Services.AddSingleton<IConfiguration>(configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.UseCors("AllowReactApp"); 

            app.MapControllers();

            app.Run();
        }
    }
}