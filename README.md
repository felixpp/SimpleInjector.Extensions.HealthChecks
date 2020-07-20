
# SimpleInjector Health Check Extensions

This library contains health check extensions for [SimpleInjector](http://www.simpleinjector.org/) library.
In order to use this open source library, the developer should know about the extended Simple Injector library.

## Usage

### Implement HealthChecks
Add the `IHealthCheck` interface to a service implementation class that needs health monitoring.

```
public class MyService : IMyService, IHealthCheck
{
	public async Task<HealthCheckResult> CheckHealth(CancellationToken ct, HealthCheckContext context)
	{
		// Health check implementation.
		return HealthCheckResult.NotImplemented();
	}
}
```

### Bootstrapping
When bootstrapping the Container, call
```
container.RegisterHealthChecks();
```

Calling the above will use the calling assembly to find all the `IHealthCheck` implementations.

### Resolve the IHealthCheckService

1. Create a `HealthCheckController` .
2. Resolve the `IHealthCheckService`.
3. Call the `IHealthCheckService.CheckHealth()` method.
4. Return the resulting `HealthReport`.

## Customization

1. You can provided an assembly for which to register the health checks : 
```
var assembly = GetSomeOtherAssemblyImplementingHealthChecks();
container.RegisterHealthChecks(assembly);
```

2. You can customize the health check registrations from the `IHealthCheckBuilder` itself :
```
container.RegisterHealthChecks(b => b
	.Register<MyHealthCheck>()
	.Register<MyOtherHealthCheck());
```