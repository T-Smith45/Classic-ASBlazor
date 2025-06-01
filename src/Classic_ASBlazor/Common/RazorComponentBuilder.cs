using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq.Expressions;
using System.Reflection;


namespace Classic_ASBlazor.Common;

/// <summary>
/// Static factory for creating RazorComponentResult instances with strongly-typed parameters.
/// </summary>
public static class RazorComponent
{
    /// <summary>
    /// Begins building a RazorComponentResult for the specified component type.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Razor component.</typeparam>
    /// <returns>A builder to fluently specify parameters.</returns>
    public static RazorComponentBuilder<TComponent> For<TComponent>() where TComponent : IComponent
    {
        return new RazorComponentBuilder<TComponent>();
    }
}

public class RazorComponentBuilder<TComponent> where TComponent : IComponent
{
    private readonly Dictionary<string, object?> _parameters = [];
    private int _statusCode = 200; // Default Return Code
    private string? _contentType = null; //"text/html";
    
    
    /// <summary>
    /// Sets a parameter for the component using a strongly-typed expression.
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter value.</typeparam>
    /// <param name="parameterSelector">An expression selecting the component's parameter property (e.g., c => c.PropertyName).</param>
    /// <param name="value">The value to assign to the parameter.</param>
    /// <returns>The same builder instance for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the selector is not a valid property expression.</exception>
    public RazorComponentBuilder<TComponent> With<TValue>(Expression<Func<TComponent, TValue>> parameterSelector, TValue value)
    {
        if (parameterSelector.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException("Selector must be a member expression (e.g., c => c.PropertyName).", nameof(parameterSelector));
        }

        var propertyInfo = memberExpression.Member as PropertyInfo;
        if (propertyInfo == null)
        {
            throw new ArgumentException("Selector must target a property.", nameof(parameterSelector));
        }

        // Optional: Add a runtime check to ensure the property is a valid Blazor parameter.
        // This is good for robustness, especially if components might be refactored without updating calls.
        if (!propertyInfo.IsDefined(typeof(ParameterAttribute), inherit: true) &&
            !propertyInfo.IsDefined(typeof(CascadingParameterAttribute), inherit: true))
        {
            throw new ArgumentException(
                $"Property '{propertyInfo.Name}' on component '{typeof(TComponent).FullName}' is not decorated with [Parameter] or [CascadingParameter]. Ensure it is a public property with a public setter.",
                nameof(parameterSelector));
        }

        _parameters[propertyInfo.Name] = value;
        return this;
    }
    /// <summary>
    /// Updates the status code (useful for 404's etc.)
    /// </summary>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public RazorComponentBuilder<TComponent>WithStatusCode(int statusCode)
    {
        _statusCode = statusCode;
        return this;
    }
    
    /// <summary>
    /// Updates the Content Type 
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public RazorComponentBuilder<TComponent>WithContentType(string contentType)
    {
        _contentType = contentType;
        return this;
    }

    
    /// <summary>
    /// Builds the RazorComponentResult with the specified parameters.
    /// </summary>
    /// <returns>A RazorComponentResult that will render the component.</returns>
    public RazorComponentResult<TComponent> Result()
    {
        return new RazorComponentResult<TComponent>(_parameters)
        {
            StatusCode = _statusCode,
            ContentType = _contentType ?? "text/html; charset=utf-8"
        };
    }

}