using JwtSample.Models;
using JwtSample.Services;
using JwtSample.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//Je r�cup�re les infos de config de jwt � partir du
// fichier appsettings.json et je stocke le tout 
// dans la classe pr�vue 
JwtOptions options = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();

//Pour pouvoir utiliser le jwtoption il faut l'injecter
builder.Services.AddSingleton(options);

//On configure l'authentication dans les services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
            o=>
            {
                //Je vais rechercher ma cl� de signature
                byte[] sKey = Encoding.UTF8.GetBytes(options.SigningKey);

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(sKey)
                };
            }
         
    );
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtSample For TechniBestDev", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", //!!!!!LOWERCASE!!!!!
        BearerFormat = "JWT", //!!!UPPERCASE!!!!
        In = ParameterLocation.Header, //Dans le Header Http

        Description = "JWT Bearer : \r\n Enter  Token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                      {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme
                                }
                            },
                            new string[] {}
                    }
                });
});
        

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
