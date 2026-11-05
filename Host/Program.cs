using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Photino.NET;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

#region Photino.NET single-instance lock and window
//app.MapFallbackToFile("index.html");

//// Start Kestrel and wait until it is actually listening.
//app.StartAsync().GetAwaiter().GetResult();
//var url = app.Services.GetRequiredService<IServer>()
//    .Features.Get<IServerAddressesFeature>()!.Addresses.First();

//try
//{
//    new PhotinoWindow()
//        .SetTitle("FixDesk")
//        .SetSize(1280, 800)
//        .Center()
//        .Load(url)
//        .WaitForClose(); // blocks until the window is closed
//}
//finally
//{
//    // Window closed => stop the service, no background process left.
//    app.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
//}
#endregion

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
