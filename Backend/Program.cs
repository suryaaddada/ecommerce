using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = new ConfigurationBuilder().SetBasePath(builder.Environment.ContentRootPath)
               .AddJsonFile("appsettings.json").Build();

            builder.Services.AddCors();
            builder.Services.AddDbContext<DBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Data")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(builder => builder

           .AllowAnyMethod()
           .AllowAnyHeader()
           .SetIsOriginAllowed(origin => true)  
           .AllowCredentials()
           );

            app.UseRouting();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
