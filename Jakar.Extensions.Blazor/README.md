# Jakar.Extensions.Blazor

**Contributions and Ideas are Welcome.**

## Overview

`Jakar.Extensions.Blazor` is a .NET library that includes a variety of extensions and helper methods designed to streamline and
enhance the development process. This package provides utility functions for common tasks, making your code cleaner and
more maintainable.

## Installation

You can install the `Jakar.Extensions.Blazor` package via [Nuget](https://www.nuget.org/packages/Jakar.Extensions.Blazor/):

```sh
dotnet add package Jakar.Extensions.Blazor
```

Alternatively, you can use the NuGet Package Manager in Visual Studio.

## Features

- `Widget` : Provides a common interface for all components. It also includes a `Errors` property `ErrorState`, and a `User` property `LoginState`. It also includes a `StateHasChanged` method for updating the UI asyncronously. Inherites from `ComponentBase`.
- `Page` : Intented for use as a page in a Blazor application. Inherits from `Widget`.
- `LoginProvider` : Allows a common interface for login and user management.
- `ModelStateProvider` : Provides a `ModelStateDictionary` for all `Widgets` to easily handle errors.
- `BlazorServices` : All in one class for blazor services -- makes it easier to inject services into components.

### License

This project is licensed under the MIT License. See the LICENSE.txt file for details.
Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.
