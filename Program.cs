using CloudinaryDotNet;
using dotenv.net;
using Microsoft.Extensions.Options;
using WebApplication1.Interface;
using WebApplication1.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<IToken , Jasonwt>();
builder.Services.AddScoped<ICloudnaryInterface, CloudnaryService>();
builder.Services.AddSingleton<IMail , EmailService>();

builder.Services.AddControllers();
builder.Services.AddCors(Options=>{
    Options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();


app.Run();

