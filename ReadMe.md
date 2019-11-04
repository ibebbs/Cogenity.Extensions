# Microsoft.Extensions.Hosting.PlugIns

Provides the ability to augment the generic host (using Microsoft.Extensions.Hosting) with additional services from dynamically loaded plugins

Similar to [Dapplo.Microsoft.Extensions.Hosting](https://github.com/dapplo/Dapplo.Microsoft.Extensions.Hosting) but with a focus on configuring plugins which provides the following advantages:

- Less time spent scanning directories
- Add a plugin more than once [with different configuration]

## Remarks

Requires .NET Core 3.0