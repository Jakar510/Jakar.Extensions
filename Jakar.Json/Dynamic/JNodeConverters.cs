// Jakar.Json
// 10/07/2026

using Jakar.Json.Converters;



namespace Jakar.Json
{
    public abstract partial class JNode
    {
        /// <summary> Reads any JSON value from <paramref name="reader"/> as a tree (JSON <c>null</c> → <see langword="null"/>). Numbers keep their text. A repeated member name fails with <see cref="JsonErrorKind.DuplicateMember"/>. </summary>
        public static bool TryRead<TReader>( ref TReader reader, out JNode? node )
            where TReader : IJsonReader, allows ref struct
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            node = null;

            switch ( reader.PeekKind() )
            {
                case JsonTokenKind.Object:
                {
                    if ( !reader.TryReadStartObject() ) { return false; }

                    JObjectNode obj = new();

                    while ( true )
                    {
                        JsonReaderCheckpoint mark = reader.Checkpoint();
                        if ( !reader.TryReadProperty(out JsonSpan name, out bool end) ) { return false; }

                        if ( end ) { break; }

                        string key = name.ToString();
                        if ( !TryRead(ref reader, out JNode? value) ) { return false; }

                        if ( obj.TryAddNew(key, value) ) { continue; }

                        reader.Rewind(mark);
                        return reader.Fail(JsonErrorKind.DuplicateMember);
                    }

                    node = obj;
                    return true;
                }

                case JsonTokenKind.Array:
                {
                    if ( !reader.TryReadStartArray() ) { return false; }

                    JArrayNode array = new();

                    while ( true )
                    {
                        if ( !reader.TryReadNextElement(out bool end) ) { return false; }

                        if ( end ) { break; }

                        if ( !TryRead(ref reader, out JNode? value) ) { return false; }

                        array.AddNew(value);
                    }

                    node = array;
                    return true;
                }

                case JsonTokenKind.String:
                    if ( !reader.TryReadString(out string? text) ) { return false; }

                    node = new JValueNode(text);
                    return true;

                case JsonTokenKind.Number:
                    if ( !reader.TryReadRawNumber(out JsonSpan number) ) { return false; }

                    node = JValueNode.FromValidatedNumberText(number.ToString());
                    return true;

                case JsonTokenKind.True or JsonTokenKind.False:
                    if ( !reader.TryReadBoolean(out bool flag) ) { return false; }

                    node = new JValueNode(flag);
                    return true;

                case JsonTokenKind.Null:
                    return reader.TryReadNull();

                default:
                    return reader.Fail(JsonErrorKind.UnexpectedToken);
            }
        }
    }
}



namespace Jakar.Json.Converters
{
    /// <summary> Any JSON value as a <see cref="JNode"/> (JSON <c>null</c> ↔ <see langword="null"/>). </summary>
    public readonly struct JsonNodeConverter : IJsonConverter<JNode?>
    {
        public static void Write<TWriter>( ref TWriter writer, scoped in JNode? value )
            where TWriter : IJsonWriter, allows ref struct => JNode.Write(value, ref writer);

        public static bool TryRead<TReader>( ref TReader reader, out JNode? value )
            where TReader : IJsonReader, allows ref struct => JNode.TryRead(ref reader, out value);
    }



    /// <summary> A <see cref="JObjectNode"/>, <see cref="JArrayNode"/> or <see cref="JValueNode"/> member: reading a different kind of value (or <c>null</c>) fails with <see cref="JsonErrorKind.InvalidValue"/>. </summary>
    public readonly struct JsonNodeConverter<TNode> : IJsonConverter<TNode>
        where TNode : JNode
    {
        public static void Write<TWriter>( ref TWriter writer, scoped in TNode value )
            where TWriter : IJsonWriter, allows ref struct => JNode.Write(value, ref writer);

        public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TNode value )
            where TReader : IJsonReader, allows ref struct
        {
            value = null;
            JsonReaderCheckpoint mark = reader.Checkpoint();
            if ( !JNode.TryRead(ref reader, out JNode? node) ) { return false; }

            if ( node is TNode typed )
            {
                value = typed;
                return true;
            }

            reader.Rewind(mark);
            return reader.Fail(JsonErrorKind.InvalidValue);
        }
    }
}
