# Chain of Responsibility Pattern in .NET

This repository demonstrates the implementation of the Chain of Responsibility (CoR) design pattern in .NET. The CoR pattern is a behavioral design pattern that delegates tasks along a chain of handlers, providing decoupling and extensibility.

## Table of Contents

- [Chain of Responsibility Pattern in .NET](#chain-of-responsibility-pattern-in-net)
  - [Table of Contents](#table-of-contents)
  - [What is the Chain of Responsibility Pattern?](#what-is-the-chain-of-responsibility-pattern)
  - [Comparison: CoR vs Multiple If-Else](#comparison-cor-vs-multiple-if-else)
    - [Multiple If-Else Statements](#multiple-if-else-statements)
    - [Chain of Responsibility Pattern](#chain-of-responsibility-pattern)
  - [When to Use CoR](#when-to-use-cor)
  - [Real-World Example in .NET Core](#real-world-example-in-net-core)
    - [Scenario: Order Processing Pipeline](#scenario-order-processing-pipeline)
  - [Solution Structure](#solution-structure)
  - [Getting Started](#getting-started)
  - [Contributing](#contributing)
  - [License](#license)
  - [Acknowledgments](#acknowledgments)

## What is the Chain of Responsibility Pattern?

The Chain of Responsibility pattern creates a chain of objects where each object decides whether to handle a request or pass it to the next object in the chain. This provides:

- **Decoupling**: The sender does not need to know who will handle the request
- **Extensibility**: Adding or removing handlers does not impact other parts of the chain

## Comparison: CoR vs Multiple If-Else

### Multiple If-Else Statements

```csharp
public class OrderProcessor
{
    public string ProcessOrder(Order order)
    {
        if (!ValidateOrder(order))
        {
            return "Order validation failed";
        }
        else if (!CheckInventory(order))
        {
            return "Insufficient inventory";
        }
        else if (!ProcessPayment(order))
        {
            return "Payment processing failed";
        }
        else if (!ShipOrder(order))
        {
            return "Shipping failed";
        }
        else
        {
            return "Order processed successfully";
        }
    }

    private bool ValidateOrder(Order order) { /*...*/ }
    private bool CheckInventory(Order order) { /*...*/ }
    private bool ProcessPayment(Order order) { /*...*/ }
    private bool ShipOrder(Order order) { /*...*/ }
}
```

**Drawbacks**:

- Tightly Coupled Logic: All processing steps are in one class
- Rigid Code: Difficult to add or remove processing steps
- Single Responsibility Violation: One class handles multiple responsibilities
- Difficult to Test: Requires testing all scenarios in one method

### Chain of Responsibility Pattern

```csharp
public abstract class OrderHandler
{
    protected OrderHandler _nextHandler;

    public void SetNext(OrderHandler nextHandler)
    {
        _nextHandler = nextHandler;
    }

    public abstract string Handle(Order order);
}

public class OrderValidationHandler : OrderHandler
{
    public override string Handle(Order order)
    {
        if (!ValidateOrder(order))
        {
            return "Order validation failed";
        }
        return _nextHandler?.Handle(order) ?? "Order processed successfully";
    }

    private bool ValidateOrder(Order order) { /*...*/ }
}

public class InventoryCheckHandler : OrderHandler
{
    public override string Handle(Order order)
    {
        if (!CheckInventory(order))
        {
            return "Insufficient inventory";
        }
        return _nextHandler?.Handle(order) ?? "Order processed successfully";
    }

    private bool CheckInventory(Order order) { /*...*/ }
}

// Additional handlers for PaymentProcessing and Shipping...
```

**Benefits**:

- Decoupled logic
- Flexible and extensible
  - Adding or removing handlers does not impact other parts of the chain
- Single Responsibility Principle compliance

## When to Use CoR

1. **Request Processing Pipelines**
2. **Validation Chains**
3. **Conditional Workflows**

## Real-World Example in .NET Core

### Scenario: Order Processing Pipeline

```csharp
// Order model
public record Order(
    Guid Id,
    string CustomerId,
    List<OrderItem> Items,
    decimal TotalAmount,
    PaymentMethod PaymentMethod,
    ShippingAddress ShippingAddress);

// Base handler
public abstract class OrderHandler
{
    protected OrderHandler _nextHandler;

    public void SetNext(OrderHandler nextHandler)
    {
        _nextHandler = nextHandler;
    }

    public abstract Task<OrderResult> HandleAsync(Order order);
}

// Validation handler
public class OrderValidationHandler : OrderHandler
{
    public override async Task<OrderResult> HandleAsync(Order order)
    {
        if (order.Items.Count == 0)
        {
            return OrderResult.Failure("Order must contain at least one item");
        }

        if (order.TotalAmount <= 0)
        {
            return OrderResult.Failure("Order total must be greater than zero");
        }

        return _nextHandler != null
            ? await _nextHandler.HandleAsync(order)
            : OrderResult.Success();
    }
}

// Inventory handler
public class InventoryCheckHandler : OrderHandler
{
    public override async Task<OrderResult> HandleAsync(Order order)
    {
        foreach (var item in order.Items)
        {
            if (!await CheckInventoryAsync(item))
            {
                return OrderResult.Failure($"Insufficient inventory for product {item.ProductId}");
            }
        }

        return _nextHandler != null
            ? await _nextHandler.HandleAsync(order)
            : OrderResult.Success();
    }

    private async Task<bool> CheckInventoryAsync(OrderItem item) { /*...*/ }
}

// Payment handler
public class PaymentProcessingHandler : OrderHandler
{
    public override async Task<OrderResult> HandleAsync(Order order)
    {
        var paymentResult = await ProcessPaymentAsync(order);
        return paymentResult.Success
            ? _nextHandler != null
                ? await _nextHandler.HandleAsync(order)
                : OrderResult.Success()
            : OrderResult.Failure(paymentResult.ErrorMessage);
    }

    private async Task<PaymentResult> ProcessPaymentAsync(Order order) { /*...*/ }
}

// Shipping handler
public class ShippingHandler : OrderHandler
{
    public override async Task<OrderResult> HandleAsync(Order order)
    {
        var shippingResult = await ScheduleShippingAsync(order);
        return shippingResult.Success
            ? OrderResult.Success()
            : OrderResult.Failure(shippingResult.ErrorMessage);
    }

    private async Task<ShippingResult> ScheduleShippingAsync(Order order) { /*...*/ }
}

// Result classes
public record OrderResult(bool Success, string? ErrorMessage = null)
{
    public static OrderResult Success() => new(true);
    public static OrderResult Failure(string errorMessage) => new(false, errorMessage);
}

public record PaymentResult(bool Success, string? ErrorMessage = null)
{
    public static PaymentResult Success() => new(true);
    public static PaymentResult Failure(string errorMessage) => new(false, errorMessage);
}

// Usage in Minimal API
app.MapPost("/orders", async (Order order) =>
{
    var validationHandler = new OrderValidationHandler();
    var inventoryHandler = new InventoryCheckHandler();
    var paymentHandler = new PaymentProcessingHandler();
    var shippingHandler = new ShippingHandler();

    validationHandler.SetNext(inventoryHandler);
    inventoryHandler.SetNext(paymentHandler);
    paymentHandler.SetNext(shippingHandler);

    return await validationHandler.HandleAsync(order);
});
```

## Solution Structure

The solution contains the following key components:

- **CoRSample.sln**: Main solution file
- **Directory.Build.props**: Centralized build properties
- **Directory.Packages.props**: Centralized package management
- **global.json**: .NET SDK version configuration
- **src/**: Contains the implementation of the Chain of Responsibility pattern
- **tests/**: Contains unit tests for the implementation

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio
3. Build and run the project

## Contributing

Contributions are welcome! Please follow the standard GitHub workflow:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspired by the blog post at [dotnet-fullstack-dev.blogspot.com](https://dotnet-fullstack-dev.blogspot.com/)
- [Unleashing Flexibility in Your .NET API: How the Chain of Responsibility Pattern Can Transform Your Item API! 🚀](https://dotnet-fullstack-dev.blogspot.com/2024/10/unleashing-flexibility-in-your-net-api.html)
- [Better Use Case for Chain of Responsibility Pattern: Is It Just a Replacement for Multiple If-Else Statements? | by DotNet Full Stack Dev | Dec, 2024 | Medium](https://dotnetfullstackdev.medium.com/better-use-case-for-chain-of-responsibility-pattern-is-it-just-a-replacement-for-multiple-if-else-e32b7c6044ae)
