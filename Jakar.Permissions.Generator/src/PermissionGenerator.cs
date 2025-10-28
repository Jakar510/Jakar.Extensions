using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;



namespace Jakar.Permissions.Generator;


[Generator(LanguageNames.CSharp)]
public sealed class PermissionGenerator : IIncrementalGenerator
{
    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        // Read options from compilation
        IncrementalValueProvider<(bool IncludeDocs, bool IncludeDebuggerDisplay, string Namespace, string RootClass)> optionsProvider = context.CompilationProvider.Select(PermissionGenUtilities.GetOptions);

        // Additional files provider (auto-discover if none explicitly provided)
        IncrementalValuesProvider<AdditionalText> additional = context.AdditionalTextsProvider;

        // Try to find explicit AdditionalFile named permissions.json
        IncrementalValuesProvider<AdditionalText> explicitJson = additional.Where(CheckText)
                                                                           .Collect()
                                                                           .SelectMany(ConsolidateText);

        // Map to text contents
        IncrementalValuesProvider<(string Path, string Text)>                                                                                                   jsonContent = explicitJson.Select(HandleText);
        IncrementalValuesProvider<((string Path, string Text) Left, (bool IncludeDocs, bool IncludeDebuggerDisplay, string Namespace, string RootClass) Right)> input       = jsonContent.Combine(optionsProvider);

        context.RegisterSourceOutput(input, ProcessOutput);
    }
    private static void ProcessOutput( SourceProductionContext context, ((string Path, string Text) Left, (bool IncludeDocs, bool IncludeDebuggerDisplay, string Namespace, string RootClass) Right) pair )
    {
        string                                                                              path = pair.Left.Path;
        string                                                                              json = pair.Left.Text;
        (bool IncludeDocs, bool IncludeDebuggerDisplay, string Namespace, string RootClass) opts = pair.Right;

        while ( PermissionGenUtilities.PendingDiagnostics.TryDequeue(out Diagnostic? diagnostic) ) { context.ReportDiagnostic(diagnostic); }

        if ( string.IsNullOrWhiteSpace(json) )
        {
            context.ReportDiagnostic(Diagnostics.Missing);
            return;
        }

        List<string> permissions = PermissionGenUtilities.ParsePermissions(json);

        if ( permissions.Count == 0 )
        {
            context.ReportDiagnostic(Diagnostics.Empty);
            return;
        }

        // Sort to keep deterministic order
        permissions.Sort(StringComparer.Ordinal);

        PermissionGenUtilities.GenerateRegistry(context, permissions);
        PermissionGenUtilities.GenerateConstants(context, permissions, opts.IncludeDocs, opts.IncludeDebuggerDisplay, opts.Namespace, opts.RootClass);
    }
    private static (string Path, string Text) HandleText( AdditionalText file, CancellationToken token ) =>
        ( file.Path, Text: file.GetText(token)
                              ?.ToString() ??
                           string.Empty );
    private static ImmutableArray<AdditionalText> ConsolidateText( ImmutableArray<AdditionalText> files, CancellationToken token ) =>
        files.Length > 0
            ? files
            : PermissionGenUtilities.AutoDiscoverFiles(token);
    private static bool CheckText( AdditionalText file ) =>
        Path.GetFileName(file.Path)
            .Equals(Constants.FILE_NAME, StringComparison.OrdinalIgnoreCase);
}
