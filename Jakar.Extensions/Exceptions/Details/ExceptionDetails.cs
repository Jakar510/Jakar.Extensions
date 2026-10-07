namespace Jakar.Extensions;


[JsonModel(typeof(JakarExtensionsContext))]
public sealed partial class ExceptionDetails : BaseClass<ExceptionDetails>, IEqualComparable<ExceptionDetails>, IJsonModel<ExceptionDetails>
{
    [JsonIgnore] public Exception?        Value           { get; private init; }
    public              JsonObject?       Data            { get; init; }
    public              string?           HelpLink        { get; init; }
    public              int               HResult         { get; init; }
    public              ExceptionDetails? Inner           { get; init; }
    public              string            Message         { get; init; } = EMPTY;
    public              string?           MethodSignature { get; init; }
    public              string?           Source          { get; init; }
    public              string[]          StackTrace      { get; init; } = [];
    public              string            Str             { get; init; } = EMPTY;
    public              MethodDetails?    TargetSite      { get; init; }
    public              string?           Type            { get; init; }


    public ExceptionDetails() { }


    /// <summary> Trim/AOT-safe: everything except <see cref="TargetSite"/>; <see cref="MethodSignature"/> is <see langword="null"/> when the method metadata isn't available. </summary>
    /// <remarks> Factories rather than constructors: System.Text.Json's generated metadata reflects over the constructors, and the method-info variant needs method metadata. </remarks>
    public static ExceptionDetails Create( Exception exception )
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new ExceptionDetails
               {
                   Value           = exception,
                   Message         = exception.Message,
                   HResult         = exception.HResult,
                   Type            = exception.GetType().FullName,
                   HelpLink        = exception.HelpLink,
                   Source          = exception.Source,
                   StackTrace      = exception.StackTrace?.SplitAndTrimLines().ToArray() ?? [],
                   MethodSignature = exception.TryGetMethodSignature(),
                   Data            = exception.GetData(),
                   Str             = exception.ToString(),
                   Inner = exception.InnerException is null
                               ? null
                               : Create(exception.InnerException)
               };
    }


    /// <summary> Also fills <see cref="TargetSite"/> (parameters, attributes), which needs method metadata the trimmer may remove. </summary>
    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")] public static ExceptionDetails CreateWithMethodInfo( Exception exception )
    {
        ArgumentNullException.ThrowIfNull(exception);
        ExceptionDetails details = Create(exception);

        return new ExceptionDetails
               {
                   Value           = details.Value,
                   Message         = details.Message,
                   HResult         = details.HResult,
                   Type            = details.Type,
                   HelpLink        = details.HelpLink,
                   Source          = details.Source,
                   StackTrace      = details.StackTrace,
                   MethodSignature = $"{exception.MethodClass()}::{exception.MethodSignature()}",
                   Data            = details.Data,
                   Str             = details.Str,
                   TargetSite      = exception.MethodInfo(),
                   Inner = exception.InnerException is null
                               ? null
                               : CreateWithMethodInfo(exception.InnerException)
               };
    }
    public static implicit operator ExceptionDetails?( Exception? e )       => TryCreate(e);
    public static implicit operator Exception?( ExceptionDetails? details ) => details?.Value;


    private static ExceptionDetails? TryCreate( [NotNullIfNotNull(nameof(exception))] Exception? exception ) => exception is not null
                                                                                                                    ? Create(exception)
                                                                                                                    : null;
    public override bool Equals( ExceptionDetails? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Type == other.Type && Source == other.Source && Message == other.Message && MethodSignature == other.MethodSignature && HelpLink == other.HelpLink && HResult == other.HResult && Equals(Inner, other.Inner) && StackTrace.Equals(other.StackTrace) && Str == other.Str && Equals(TargetSite, other.TargetSite) && Equals(Data, other.Data);
    }
    public override bool Equals( object? obj ) => ReferenceEquals(this, obj) || ( obj is ExceptionDetails other && Equals(other) );
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(Type);
        hashCode.Add(Source);
        hashCode.Add(Message);
        hashCode.Add(MethodSignature);
        hashCode.Add(HelpLink);
        hashCode.Add(HResult);
        hashCode.Add(Inner);
        hashCode.Add(StackTrace);
        hashCode.Add(Str);
        hashCode.Add(TargetSite);
        hashCode.Add(Data);
        return hashCode.ToHashCode();
    }
    public override int CompareTo( ExceptionDetails? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int typeComparison = string.Compare(Type, other.Type, StringComparison.Ordinal);
        if ( typeComparison != 0 ) { return typeComparison; }

        int sourceComparison = string.Compare(Source, other.Source, StringComparison.Ordinal);
        if ( sourceComparison != 0 ) { return sourceComparison; }

        int methodSignatureComparison = string.Compare(MethodSignature, other.MethodSignature, StringComparison.Ordinal);
        if ( methodSignatureComparison != 0 ) { return methodSignatureComparison; }

        int messageComparison = string.Compare(Message, other.Message, StringComparison.Ordinal);
        if ( messageComparison != 0 ) { return messageComparison; }

        int strComparison = string.Compare(Str, other.Str, StringComparison.Ordinal);
        if ( strComparison != 0 ) { return strComparison; }

        int hResultComparison = HResult.CompareTo(other.HResult);
        if ( hResultComparison != 0 ) { return hResultComparison; }

        int helpLinkComparison = string.Compare(HelpLink, other.HelpLink, StringComparison.Ordinal);
        if ( helpLinkComparison != 0 ) { return helpLinkComparison; }

        return Comparer<ExceptionDetails?>.Default.Compare(Inner, other.Inner);
    }


    public static bool operator <( ExceptionDetails?  left, ExceptionDetails? right ) => Comparer<ExceptionDetails>.Default.Compare(left, right) < 0;
    public static bool operator >( ExceptionDetails?  left, ExceptionDetails? right ) => Comparer<ExceptionDetails>.Default.Compare(left, right) > 0;
    public static bool operator <=( ExceptionDetails? left, ExceptionDetails? right ) => Comparer<ExceptionDetails>.Default.Compare(left, right) <= 0;
    public static bool operator >=( ExceptionDetails? left, ExceptionDetails? right ) => Comparer<ExceptionDetails>.Default.Compare(left, right) >= 0;
    public static bool operator ==( ExceptionDetails? left, ExceptionDetails? right ) => EqualityComparer<ExceptionDetails>.Default.Equals(left, right);
    public static bool operator !=( ExceptionDetails? left, ExceptionDetails? right ) => !EqualityComparer<ExceptionDetails>.Default.Equals(left, right);
}
