using Microsoft.EntityFrameworkCore;
using Backend.Data; 
using Backend.Model;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Backend.Service;
using Backend.GraphQL.Queries;
using Backend.GraphQL.Mutations;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
//Add DbContext'
builder.Services.AddDbContext<ApplicationDbContext>(
    option => option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//Add Controllers
builder.Services.AddControllers();
//Add Cors for fontend connection
builder.Services.AddCors(option => option.AddPolicy("AllowFrontend", policy =>
{
    policy.WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod();
}));
//Add AuthService
builder.Services.AddScoped<IAuthService, AuthService>();
//Add Product Services
builder.Services.AddScoped<IProductService, ProductService>();
//Add Shipping Detail Service
builder.Services.AddScoped<IShippingDetailService, ShippingDetailService>();
//Add Payment Service
builder.Services.AddScoped<IPaymentService, PaymentService>();
//Add Order Service
builder.Services.AddScoped<IOrderService, OrderService>();

//Add GraphQL
builder.Services.AddGraphQLServer()
    .AddQueryType()
    .AddMutationType()
    .AddTypeExtension<CustomerQuery>()
    .AddTypeExtension<CustomerMutation>()
    .AddTypeExtension<ProductQuery>()
    .AddTypeExtension<ProductMutation>()
    .AddTypeExtension<ShippingDetailsQuery>()
    .AddTypeExtension<ShippingDetailMutation>()
    .AddTypeExtension<PaymentMutation>()
    .AddTypeExtension<PaymentQuery>()
    .AddTypeExtension<OrderMutation>();

var app =builder.Build();

//Apply migrations and seed admin user
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    DbSeederAdmin.SeedAdminUser(dbContext);
}

app.UseCors("AllowFrontend");
app.UseRouting();
//Map Controllers
app.MapControllers();


//Map GraphQL
app.MapGraphQL();


app.Run();


