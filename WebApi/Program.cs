

using Infrastructure;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOpenApi();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();

            }


            //app.UseSwagger(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ABC School API");
            //});


            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseInfrastructure(); 


            app.MapControllers();

            app.Run();
        }
    }
}
