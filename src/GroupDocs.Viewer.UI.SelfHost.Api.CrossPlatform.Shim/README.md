# GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform

> **DEPRECATED** - use [GroupDocs.Viewer.UI.SelfHost.Api](https://www.nuget.org/packages/GroupDocs.Viewer.UI.SelfHost.Api) instead.

As of 26.8.0, `GroupDocs.Viewer` ships per-TFM runtime packages that are cross-platform on Windows,
Linux, and macOS. `GroupDocs.Viewer.UI.SelfHost.Api` is therefore cross-platform too, and this
package has no reason to exist.

This version contains no assemblies. It only forwards to `GroupDocs.Viewer.UI.SelfHost.Api` and
emits build warning `GDVUI0001`.

## Migrating

Replace the reference:

```diff
-<PackageReference Include="GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform" Version="26.8.0" />
+<PackageReference Include="GroupDocs.Viewer.UI.SelfHost.Api" Version="26.8.0" />
```

No code changes are needed - the namespaces, the `AddGroupDocsViewerSelfHostApi()` entry point, and
the configuration API are all unchanged.

If you need to ship before migrating, silence the warning with `<NoWarn>GDVUI0001</NoWarn>`.
