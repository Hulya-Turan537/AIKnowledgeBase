
using AIKnowledgeBase.Core.Interfaces;
using AIKnowledgeBase.Data;
using AIKnowledgeBase.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AIKnowledgeBase.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext'i sisteme tanýtýyoruz
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //sözleþme ile gerçeði birbirine baðlýyoruz
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //UnitOfWork'ü sisteme tanýtýyoruz, böylece istediðimiz yerde kullanabiliriz (örneðin, controllerlarda)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>(); 

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // builder.Services.AddOpenApi();

            builder.Services.AddAutoMapper(typeof(MapProfile)); // AutoMapper'ý sisteme tanýtýyoruz, MapProfile sýnýfýnda tanýmladýðýmýz eþlemeleri kullanarak nesneler arasýnda dönüþüm yapabiliriz

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
