using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// data base connection services 

#if DEBUG
builder.Services.AddDbContext<AppDataContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection")));
#else
builder.Services.AddDbContext<AppDataContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endif

//todo : study cors 
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder => policyBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
builder.Services.AddAutoMapper(typeof(DTOMapper));

// ignore the shit loop man we know what we doing ...
builder.Services.AddControllers().AddNewtonsoftJson(options=> options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
  
}
app.UseSwagger();
app.UseSwaggerUI(SwaggerUiOptions =>
{
    SwaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json","ValravnsServer API");
    SwaggerUiOptions.RoutePrefix = string.Empty;
} );

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();