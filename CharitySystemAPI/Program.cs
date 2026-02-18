using CharitySystem.Infrastructure;
using CharitySystem.Domain.Enums;
using CharitySystem.Domain.Entities;


Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Put your token here"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles(); 
app.UseStaticFiles();  
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CharitySystem.Infrastructure.Data.AppDbContext>();

    if (context.Users.FirstOrDefault() == null)
    {
        Console.WriteLine("--> No users found. Seeding Users...");

        var adminUser = new User
        {
            Fullname = "Admin User",
            Username = "admin",
            PhoneNumber = "0500000000",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin8098"),
            Gender = Gender.Male,
            Role = UserRole.Admin,
            AccountStatus = AccountStatus.Active,
            CreatedAt = DateTime.Now
        };

        var managerUser = new User
        {
            Fullname = "Warehouse Manager",
            Username = "whmanager",
            PhoneNumber = "0511111111",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("whmanager8098"),
            Gender = Gender.Male,
            Role = UserRole.WarehouseManager,
            AccountStatus = AccountStatus.Active,
            CreatedAt = DateTime.Now
        };

        context.Users.AddRange(adminUser, managerUser);
        context.SaveChanges();

        Console.WriteLine("--> Seed Data Created Successfully:");
        Console.WriteLine("    [Admin]: user: admin, pass: admin123");
        Console.WriteLine("    [Manager]: user: manager, pass: manager123");
    }
}
app.Run();