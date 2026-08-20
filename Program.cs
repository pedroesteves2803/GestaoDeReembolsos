using System.Text;
using GestaodeReembolsos;
using GestaodeReembolsos.Data;
using GestaodeReembolsos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Informe o token JWT"
        });
    
    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});


var app = builder.Build();
LoadConfiguration(app);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();

void LoadConfiguration(WebApplication app)
{
    Configuration.JwtKey = app.Configuration.GetSection("Authentication").GetValue<string>("JwtKey")
                           ?? throw new InvalidOperationException("JwtKey não configurada.");
    
    Configuration.ApiKeyName = app.Configuration.GetSection("Authentication").GetValue<string>("ApiKeyName")
                               ?? throw new InvalidOperationException("ApiKeyName não configurada.");

    Configuration.ApiKey = app.Configuration.GetSection("Authentication").GetValue<string>("ApiKey")
                           ?? throw new InvalidOperationException("ApiKey não configurada.");
}

void ConfigureServices(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<GestaoDeReembolsoContext>(options => options.UseSqlServer(connectionString));
    builder.Services.AddTransient<ServicoToken>();
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var jwtKey = builder.Configuration.GetSection("Authentication").GetValue<string>("JwtKey")
                 ?? throw new InvalidOperationException("JwtKey não configurada.");
    
    var key = Encoding.ASCII.GetBytes(jwtKey);    
    
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder
        .Services
        .AddControllers();
    // .ConfigureApiBehaviorOptions(options =>
    // {
    //     options.SuppressModelStateInvalidFilter = true;
    // });
    // .AddJsonOptions(x =>
    // {
    //     // x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    //     // x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    // });
}
