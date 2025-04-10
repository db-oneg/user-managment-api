
using UserManagmentApi.Factories;
using UserManagmentApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IUserFactory, UserFactory>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddMemoryCache();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
 {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
