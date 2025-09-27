
using Healthcare_Appointment_System.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Healthcare_Appointment_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAutoMapper(typeof(AppointmentProfile));
            builder.Services.AddAutoMapper(typeof(ClinicProfile));
            builder.Services.AddAutoMapper(typeof(DoctorProfile));
            builder.Services.AddAutoMapper(typeof(PatientProfile));

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<Models.HealthcareAppointmentSystemContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen( c => {

                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Healthcare Appointment System API",
                    Version = "v1",
                    Description = "API for managing doctors, patients, appointments, and clinics",
                    Contact = new OpenApiContact {
                        Name = "Healthcare Dev Team",
                        Email = "support@healthcare.com"
                    }
                });
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if(File.Exists(xmlPath)) {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Healthcare Appointment System API v1");
                    c.RoutePrefix = string.Empty; 
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
