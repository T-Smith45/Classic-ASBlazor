using Classic_ASBlazor.Common;
using Classic_ASBlazor.Components;
using Classic_ASBlazor.Components.Partial;

namespace Classic_ASBlazor;

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