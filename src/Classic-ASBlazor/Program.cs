using Simple_Razor_Components.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents();

var app = builder.Build();
app.UseStaticFiles(); // To Serve JS/CSS at a later time

if(true)
    app.UseHtmxRequestBlocking("/htmx-only/");

app.MapDefaultEndpoints();
if (true)
    app.HtmxEndpoints();
else
    app.SampleEndpoints();


app.Run();