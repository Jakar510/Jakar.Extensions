# Jakar.Extensions

**Contributions and Ideas are Welcome.**

## Overview

`Jakar.Extensions` is a .NET library that includes a variety of extensions and helper methods designed to streamline and
enhance the development process. This package provides utility functions for common tasks, making your code cleaner and
more maintainable.

## Installation

You can install the `Jakar.Extensions` package via [Nuget](https://www.nuget.org/packages/Jakar.Extensions/):

```sh
dotnet add package Jakar.Extensions
```

Alternatively, you can use the NuGet Package Manager in Visual Studio.

## Features and Hhighlights of the extensions provided by this package.

### Base classes

- `ObservableClass` : A base class that implements the `INotifyPropertyChanged` and `INotifyPropertyChanging` interface.
- `ObservableRecord` : A base record that implements the `INotifyPropertyChanged` and `INotifyPropertyChanging` interface.

### Collection

- `ObservableConcurrentDictionary` : A thread-safe dictionary that supports change notifications.
- `ObservableDictionary` : A dictionary that supports change notifications.
- `ConcurrentObservableCollection` : A thread-safe collection that supports change notifications.
- `ConcurrentHashSet` : A thread-safe hash set.
- `ConcurrentDeque` : A thread-safe queue.
- `FixedSizedDeque` : A fixed-size deque.
- `FixedSizedQueue` : A fixed-size queue.

### HTTP

- `WebRequester` : A wrapper for `System.Net.Http.HttpClient` that simplifies the process of making HTTP requests and accepts an `IHostInfo` object to provide the base Uri. Also supports automatic retries.
- `WebHandler` : An intermediate class that provides a way to handle the response from the `WebRequester` class.
- `WebResponse` : The representation of the response from the `WebRequester` class.
- `Status` : An enumration of the possible HTTP status codes.
- `MimeType` : An enumeration of the possible MIME types.
- `MimeTypeNames` : The equivalent string representation of the `MimeType` enumration.

### Miscellaneous

- `SecuredString` : A class that provides a way to store sensitive data in memory securely.
- `Validate` : A static class that provides a way to validate various strings and states.
- `Tasks` : A static class that provides a way to handle tasks such as SafeFireAndForget.
- `TypeExtensions` : A static class that provides methods to see if they are ISet, IList or IDictionary and get the types or see if its a built in class or struct, or if its nullable.
- `IDataProtector` and `IDataProtectorProvider` : A class to encrypt and decrypt strings.
- `ErrorOrResult` : A struct that represents a result or an error.
- `LocalFile` : A class that provides a way to easily handle local files such as reading and writing.
- `LocalDirectory` : A class that provides a way to easily manage local directories such as creating and deleting, and getting files.
- `PasswordValidator` : A class that provides an efficient way to validate passwords.
- `LoginRequest` : A standard way to handle login requests.

### AsyncLinq

Many of the following methods have overloads:
- `Average`
- `AsAsyncEnumerable`
- `IsEmpty`
- `ToHashSet`
- `ToList`
- `GetArray`
- `ToArray`
- `Sorted`
- `ToObservableCollection`
- `Enumerate`
- `EnumeratePairs`
- `Append`
- `Prepend`
- `Cast`
- `CastSafe`
- `Distinct`
- `DistinctBy`
- `Skip`
- `SkipLast`
- `SkipWhile`
- `Where`
- `Select`
- `Consolidate`
- `ConsolidateUnique`
- `WhereNotNull`
- `WhereNotNullAsync`
- `All`
- `Any`
- `ForEachAsync`
- `ForEachParallelAsync`
- `WhenAllParallelAsync`
- `ForEach`
- `ForEachParallel`
- `Max`
- `Min`
- `Add`
- `AddOrUpdate`
- `Remove`
- `TryAdd`
- `AddDefault`
- `Random`
- `RandomValues`
- `RandomKeys`
- `Range`
- `First`
- `FirstOrDefault`
- `Last`
- `LastOrDefault`
- `Single`
- `SingleOrDefault`

### Hashes

Many of the following methods have overloads:
- `GetHash`
- `Hash128`
- `Hash`
- `ToBytes`
- `Hash_MD5`
- `Hash_SHA1`
- `Hash_SHA256`
- `Hash_SHA384`
- `Hash_SHA512`
- `HashAsync`
- `HashAsync_MD5`
- `HashAsync_SHA1`
- `HashAsync_SHA256`
- `HashAsync_SHA384`
- `HashAsync_SHA512`

### Types

Many of the following methods have overloads:
- `GetJsonIsRequired`
- `GetJsonProperty`
- `GetJsonKey`
- `IsAnyBuiltInType`
- `IsBuiltInNullableType`
- `IsBuiltInType`
- `IsGenericType`
- `IsNullableType`
- `IsInitOnly`
- `IsEqualType`
- `IsOneOfType`
- `Construct`
- `HasInterface`
- `IsCollection`
- `IsDictionary`
- `IsDictionaryEntry`
- `IsKeyValuePair`
- `IsList`
- `IsSet`
- `MethodInfo`
- `GetParameterInfo`
- `MethodName`
- `MethodSignature`
- `MethodClass`
- `IsNullable`
- `TryGetUnderlyingEnumType`

### Spans

Many of the following methods have overloads:
- `Average`
- `ContainsAny`
- `ContainsAnyExcept`
- `IndexOfAnyExcept`
- `IndexOfAny`
- `LastIndexOfAny`
- `LastIndexOfAnyExcept`
- `Contains`
- `ContainsAll`
- `ContainsNone`
- `EndsWith`
- `StartsWith`
- `AsBytes`
- `IsNullOrWhiteSpace`
- `SequenceEqualAny`
- `SequenceEqual`
- `LastIndexOf`
- `Enumerate`
- `AsSpan`
- `AsReadOnlySpan`
- `CopyTo`
- `TryCopyTo`
- `CreateSpan`
- `CreateValue`
- `Create`
- `QuickSort`
- `First`
- `FirstOrDefault`
- `All`
- `Any`
- `Single`
- `SingleOrDefault`
- `Where`
- `WhereValues`
- `Select`
- `SelectValues`
- `Count`
- `Join`
- `Max`
- `ToMemoryStream`
- `AsMemory`
- `AsReadOnlyMemory`
- `AsArraySegment`
- `TryAsSegment`
- `ToMemory`
- `ToReadOnlyMemory`
- `ConvertToString`
- `Min`
- `As`
- `Replace`
- `RemoveAll`
- `Slice`
- `Sum`
- `Trim`
- `TrimEnd`
- `TrimStart`
- `ClampStart`
- `ClampEnd`

### License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE.txt) file for details.
Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.
