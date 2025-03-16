using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using WebAPI_Code_First.Data;
using WebAPI_Code_First.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Features;
using WebAPI_Code_First.Interface;
using System.Text;  // 🔹 Add this for Swagger Security
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS (Dynamic Allowed Origins)**
//var allowedOrigins = builder.Environment.IsDevelopment()? "*"    : builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        //if (allowedOrigins == "*")
        //{
        //    policy.AllowAnyOrigin() // ✅ Allow everything in development
        //          .AllowAnyMethod()
        //          .AllowAnyHeader();
        //}
        //else
        //{
        //    policy.WithOrigins(allowedOrigins) // ✅ Restrict in production
        //          .AllowAnyMethod()
        //          .AllowAnyHeader()
        //          .AllowCredentials();
        //}
       
            policy.AllowAnyOrigin() // ✅ Allow everything in development
                  //.AllowCredentials() // ✅ Allows cookies to be sent
                  .AllowAnyMethod()
                  .AllowAnyHeader();
         
    });
});

//-- Add Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//-- for file upload
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // Set limit to 50MB
});

//-- Dependency Injection (Register Services)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

//-- Generate RSA Key Pair for JWT Signing (Persist this in a secure location)
RSA rsa = RSA.Create();
var rsaSecurityKey = new RsaSecurityKey(rsa);

//-- Configure JWT Authentication with RSA
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = rsaSecurityKey,//-- config with RSA
            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),//-- config with HMAC 
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

//-- Register RSA instance as Singleton
builder.Services.AddSingleton<RSA>(rsa);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();  // Needed for Swagger

//--  Configure Swagger with JWT Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your JWT token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Code First API");
    });
}

var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Assset/UploadedFiles");
var profileFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Assset/ProfilePicture");

// ✅ Ensure directories exist before the app starts
if (!Directory.Exists(uploadFolderPath))
{
    Directory.CreateDirectory(uploadFolderPath);
}
if (!Directory.Exists(profileFolderPath))
{
    Directory.CreateDirectory(profileFolderPath);
}
// ✅ Serve Static Files After Ensuring Directories Exist
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadFolderPath),
    RequestPath = "/uploads"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(profileFolderPath),
    RequestPath = "/profile"
});

////-- Serve Static Files (For Uploaded Images) 
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Assset/UploadedFiles")),
//    RequestPath = "/uploads"
//});

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Assset/ProfilePicture")),
//    RequestPath = "/profile"
//});


//-- Configure Middleware
//app.UseHttpsRedirection();
//app.UseCors("AllowFrontend"); // Apply CORS Here
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();
//app.Run();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend"); // Apply CORS before authentication

app.UseRouting(); // Enable route matching
app.UseAuthentication(); // Ensure authentication is applied
app.UseAuthorization(); // Apply authorization

app.UseEndpoints(endpoints => {
    endpoints.MapControllers(); // Execute API controller routes
});

app.Run();
