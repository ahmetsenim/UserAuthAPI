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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddControllers().AddFluentValidation(fv => {
    fv.RegisterValidatorsFromAssemblyContaining<GetOTPRequestValidation>();
    fv.RegisterValidatorsFromAssemblyContaining<UserLoginRequestValidation>();
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
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
