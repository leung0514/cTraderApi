# OpenAPI.Net (Rebuild for .NET 8)

This is a rebuild of the [original cTrader OpenAPI.Net library](https://github.com/spotware/OpenAPI.Net) upgraded to .NET 8.

## Purpose of this Project

This repository was created to address a specific issue in the original [cTrader.OpenAPI.Net NuGet package](https://www.nuget.org/packages/cTrader.OpenAPI.Net/) which is missing the `ProtoOAGetPositionUnrealizedPnLReq` message type. This issue has existed for over a year without being fixed, so this project rebuilds the package with the missing type included and also upgrades from .NET 6 to .NET 8.

## Original Project

For the complete original project including samples and documentation, please visit:
- Original Repository: [https://github.com/spotware/OpenAPI.Net](https://github.com/spotware/OpenAPI.Net)
- Documentation: [https://spotware.github.io/OpenAPI.Net/](https://spotware.github.io/OpenAPI.Net/)

## Changes in this Project

- Included `ProtoOAGetPositionUnrealizedPnLReq` message type that exists in the source but was missing from the published NuGet package
- Upgraded from .NET 6 to .NET 8
- Maintained compatibility with the original API
- Focused only on the core library functionality

## Dependencies

* [protobuf](https://github.com/protocolbuffers/protobuf)
* [Reactive](https://github.com/dotnet/reactive)
* [websocket-client](https://github.com/Marfusios/websocket-client)

## License

Licensed under the [MIT license](LICENSE).

Copyright (c) 2021 Spotware  
Copyright (c) 2025 leung0514