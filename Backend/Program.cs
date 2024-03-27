

using Backend.Data;

using Backend.Models;

using Microsoft.EntityFrameworkCore;

using Microsoft.OpenApi.Models;

using Microsoft.AspNetCore.Http;



var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version())));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>

{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });

});



builder.Services.AddCors(options =>

{

    options.AddPolicy("AllowFrontend",

        builder => builder

            .AllowAnyOrigin()

            .AllowAnyMethod()

            .AllowAnyHeader());

});



builder.Services.AddAuthorization();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));



// Add IHttpContextAccessor service 

builder.Services.AddHttpContextAccessor();



var app = builder.Build();



if (app.Environment.IsDevelopment())

{

    app.UseDeveloperExceptionPage();

    app.UseSwagger();

    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1"));

}



app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();