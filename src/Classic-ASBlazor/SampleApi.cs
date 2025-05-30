using Microsoft.AspNetCore.Http.HttpResults;
using Simple_Razor_Components.Components;

namespace Simple_Razor_Components.Common;

public static class SampleApi
{
    public static void SampleEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapGet("/", () => RazorComponent.For<Home>().Result());
        
        app.MapGet("/sample", () => RazorComponent.For<WithInfo>()
            .With(tp => tp.Title,"Strongly Typed Title")
            .With(tp => tp.Info,"Strongly Typed Info")
            .Result()
        );

        app.MapGet("/sample-weak", () 
            => new RazorComponentResult<WithInfo>(new { Title = "Weakly Typed Title",Info = "Weakly Typed Info"}));
        
    }
}