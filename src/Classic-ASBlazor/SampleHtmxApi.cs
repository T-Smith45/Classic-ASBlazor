using Simple_Razor_Components.Components;
using Simple_Razor_Components.Components.Partial;

namespace Simple_Razor_Components.Common;

public static class SampleHtmxApi
{
    public static void HtmxEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => RazorComponent.For<Home>().Result());
        
        app.MapGet("/sample", () => RazorComponent.For<WithInfo>()
            .With(tp => tp.Title,"Strongly Typed Title")
            .With(tp => tp.Info,"Strongly Typed Info")
            .Result()
        );
        
        app.MapGet("/htmx-only/counter", (int val) => RazorComponent.For<Counter>()
            .With(c => c.count, val + 1)
            .Result());
    }
}