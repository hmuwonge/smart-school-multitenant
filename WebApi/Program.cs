using Infrastructure;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            
            // jwt secrete configs
           var jwtConfig = builder.Services.GetJwtSettings(builder.Configuration);
            builder.Services.AddJwtAuthentication(jwtConfig);
            var app = builder.Build(); 

            // Database Seeder
            await app.Services.AddDatabaseInitializerAsync();

            app.UseHttpsRedirection();
            app.UseInfrastructure(); 
            app.MapControllers();
            app.Run();
        }
    }
}
