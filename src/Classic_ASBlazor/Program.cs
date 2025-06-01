using Classic_ASBlazor;
using Classic_ASBlazor.Common;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents();

var app = builder.Build();
app.UseStaticFiles(); // To Serve JS/CSS at a later time

#if(!Blazor)
app.UseHtmxRequestBlocking("/htmx-only/");
#endif

app.MapDefaultEndpoints();
#if(Blazor)
app.SampleEndpoints();
#else
app.HtmxEndpoints();
#endif

app.Run();