# Jakar.Extensions.Blazor

Base components and services for Blazor apps built on [Jakar.Extensions](https://www.nuget.org/packages/Jakar.Extensions/). **Contributions and ideas are welcome.**

## Installation

```sh
dotnet add package Jakar.Extensions.Blazor
```

## Features

- `AddBlazorServices(...)`: registers the package's services, including authorization and authentication options.
- `Widget`: a `ComponentBase` with the cascading model error state (`State`, a `ModelErrorState`) and login state (`User`, a `LoginUserState`).
- `Page`: a `Widget` for routable pages, with `BlazorServices` injected and a `Title` parameter.
- `LoginUserState` / `ILoginUserState`: the signed-in user, shared with every component as a cascading value.
- `ModelErrorState` / `IModelErrorState`: model and validation errors shared with every component.
- `BlazorServices`: one injectable bundle of common services: protected local and session storage, `NavigationManager`, and the Blazored, Radzen and MudBlazor services.
- `NotifyPropertyChangedCascadingValueSource<T>` and the `CascadingValueNameExtensions`: cascading values that re-render subscribers when the value raises `PropertyChanged`.

## Versioning

9.1.0 predates the repository's versioning scheme, `<.NET major>.<library major>.<library minor>`; the next release moves to `10.x`. See the [repository README](https://github.com/Jakar510/Jakar.Extensions#versioning).

## License

MIT. See [LICENSE.txt](https://github.com/Jakar510/Jakar.Extensions/blob/master/LICENSE.txt). Contributions are welcome: open an issue or a pull request on [GitHub](https://github.com/Jakar510/Jakar.Extensions).
