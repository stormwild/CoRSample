# Notes

## EnableConfigurationBindingGenerator

The `EnableConfigurationBindingGenerator` is an MSBuild property introduced in .NET 8 that enables compile-time generation of configuration binding code. This feature is particularly useful for scenarios where reflection-based configuration binding is not suitable, such as ahead-of-time (AOT) compilation.

When you set `EnableConfigurationBindingGenerator` to `true` in your project file, the compiler generates source code that intercepts specific configuration binding calls and replaces them with generated code. This approach eliminates the need for reflection, making the application more efficient and compatible with AOT scenarios.

Here's how you can enable it in your project file:

```xml
  <!-- Analyzer configurations -->
  <PropertyGroup>
    <!-- Enables source generator for configuration binding -->
    <!-- Use case: Improves performance of configuration binding in Minimal APIs -->
    <EnableConfigurationBindingGenerator>true</EnableConfigurationBindingGenerator>
  </PropertyGroup>
```

Once enabled, the generated source code will handle configuration binding for APIs from classes like `Microsoft.Extensions.Configuration.ConfigurationBinder`, `Microsoft.Extensions.DependencyInjection.OptionsBuilderConfigurationExtensions`, and `Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions`.

This feature provides a more performant and trim-friendly way to bind configuration settings in .NET applications.

If you have any specific questions or need further details, feel free to ask!

### References

- [Compile-time configuration source generation - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-generator)
- [docs/docs/core/extensions/configuration-generator.md at main · dotnet/docs](https://github.com/dotnet/docs/blob/main/docs/core/extensions/configuration-generator.md)

## Central Package Management Rules

This project uses central package management for NuGet package versions.

### Configuration

- Central package management is configured in `Directory.Packages.props`
- All package versions are managed centrally
- Projects reference packages without specifying versions

### Rules

1. All NuGet package versions must be defined in `Directory.Packages.props`
2. Projects must not specify package versions directly
3. Package versions should be updated centrally
4. Use the latest stable versions unless specific version is required
5. Maintain version consistency across all projects

### Benefits

- Single source of truth for package versions
- Consistent versions across all projects
- Easier version updates and maintenance
- Reduced version conflicts
