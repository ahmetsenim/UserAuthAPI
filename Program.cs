using UserAuthAPI.Services.Abstract;
using UserAuthAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserAuthAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using FluentValidation.AspNetCore;
using FluentValidation;
using UserAuthAPI.Models.Concrete;
using UserAuthAPI.Models.Dtos;
using UserAuthAPI.Models.Validations;
using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.DataAccess.EntityFramework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Services
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAccessTokenService, AccessTokenService>();
builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
#endregion

builder.Services.AddControllers().AddFluentValidation(fv => {
    fv.RegisterValidatorsFromAssemblyContaining<GetOTPRequestValidation>();
    fv.RegisterValidatorsFromAssemblyContaining<UserLoginRequestValidation>();
    fv.RegisterValidatorsFromAssemblyContaining<RegisterUserRequestValidation>();
});
builder.Services.AddControllers().AddFluentValidation().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = ErrorMessageCustomize.MakeValidationResponse;
});

builder.Services.AddDbContext<ProjectDbContext>(x => x.UseSqlServer(builder.Configuration["ConnectionStrings:MsSqlConnection"]));


#region Repositories
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IOTPRepository, OTPRepository>();
builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
#endregion

//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IOTPRepository, OTPRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["AppSettings:ValidIssuer"],
        ValidAudience = builder.Configuration["AppSettings:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "User.API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Token giriniz : ",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
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
