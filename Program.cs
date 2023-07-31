using buddyUp.Data;
using buddyUp.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpsRedirection(opt => opt.HttpsPort = 443);

builder.Services.AddCors();

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseSqlServer(builder.Configuration["SqlServer:ConnectionString"]
    options.UseNpgsql(builder.Configuration["PostgreSql:ConnectionString"]
    ));

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWT"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"]);

    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // for DEVELOPING PURPUSES only put false
        ValidateAudience = false, // for DEVELOPING PURPUSES only put false
        RequireExpirationTime = false, // for DEVELOPING PURPUSES only put false --needs to be updated when refresh token is added
        ValidateLifetime = true
    };
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
options.SignIn.RequireConfirmedEmail = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
//builder.Services.AddScoped<IProfileRespository, ProfileRespository>();
// puede ser pa?! puede ser?!
//builder.Services.Configure<IdentityOptions>(
//    options =>
//    {
//        options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
//        options.ClaimsIdentity.UserIdClaimType = ClaimTypes.Role;
//        options.ClaimsIdentity.UserIdClaimType = ClaimTypes.Email;
//        options.ClaimsIdentity.UserIdClaimType = "Id";
//    });
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var app = builder.Build();

app.UseForwardedHeaders();//cuidado

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{  
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(options => options
.WithOrigins(new[] {"http://localhost:3000", "http://localhost:5173", 
    "http://127.0.0.1:5173", "https://buddyup-client-one.vercel.app", "https://buddyup-1b3fd.web.app" } )
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
