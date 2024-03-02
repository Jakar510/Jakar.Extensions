using System.Diagnostics.CodeAnalysis;
using Jakar.Extensions;



namespace Jakar.SqlBuilder;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public record struct EasySqlBuilder()
{
    private readonly StringBuilder _sb          = new(10240);
    private          bool          _needToClose = false;

    public string Result => ToString();


    internal EasySqlBuilder AddRange<T>( char separator, IEnumerable<string> names ) => AddRange( separator, names.Select( KeyWords.GetName<T> ) );
    internal EasySqlBuilder AddRange( char separator, IEnumerable<string> names )
    {
        _sb.AppendJoin( separator, names );

        return Space();
    }
    internal EasySqlBuilder AddRange<T>( string separator, IEnumerable<string> names ) => AddRange( separator, names.Select( KeyWords.GetName<T> ) );
    internal EasySqlBuilder AddRange( string separator, IEnumerable<string> names )
    {
        _sb.AppendJoin( separator, names );
        return Space();
    }


    internal EasySqlBuilder Append( string value )
    {
        _sb.Append( value );
        return this;
    }
    internal EasySqlBuilder Add( char c )
    {
        _sb.Append( c );
        return this;
    }
    internal EasySqlBuilder Add( string          value ) => Space().Append( value ).Space();
    internal EasySqlBuilder Add( params string[] names ) => AddRange( ' ', names );


    internal EasySqlBuilder NewLine()
    {
        _sb.AppendLine();
        return this;
    }


    internal EasySqlBuilder AggregateFunction( char func = '*' )
    {
        _sb.Append( func );
        return Space();
    }
    internal EasySqlBuilder AggregateFunction( string func, string? columnName = default )
    {
        _sb.Append( func );
        Begin();

        if ( columnName is null ) { _sb.Append( '*' ); }
        else { _sb.Append( columnName ); }

        End();

        return Space();
    }


    internal EasySqlBuilder Star()  => Add( '*' );
    internal EasySqlBuilder Comma() => Add( ',' );
    internal EasySqlBuilder Space() => Add( ' ' );


    internal EasySqlBuilder Begin()
    {
        _needToClose = true;
        return Add( '(' );
    }
    internal EasySqlBuilder End()
    {
        _needToClose = false;
        return Add( ')' );
    }
    internal EasySqlBuilder VerifyParentheses()
    {
        if ( _needToClose ) { End(); }

        return this;
    }


    public override string ToString()
    {
        VerifyParentheses();
        string result = _sb.Append( ';' ).ToString();


        // int start = result.LastIndexOf('(');
        // int end   = result.LastIndexOf(')');
        //
        // if ( start >= 0 )
        // {
        //     if ( end < start ) { throw new FormatException("Should not occur, no start ("); }
        // }
        // else if ( end >= 0 ) { throw new FormatException("Should not occur, no start ("); }


        return result.IsBalanced()
                   ? result
                   : throw new FormatException( $"""
                                                 String is not balanced! "{result}"
                                                 """ );
    }


    public SelectClauseBuilder<EasySqlBuilder> Select() => new(this, ref this);


    public SelectClauseBuilder<EasySqlBuilder> Union()
    {
        Add( "UNION" );
        return new SelectClauseBuilder<EasySqlBuilder>( this, ref this );
    }
    public SelectClauseBuilder<EasySqlBuilder> UnionAll()
    {
        Add( "UNION ALL" );
        return new SelectClauseBuilder<EasySqlBuilder>( this, ref this );
    }


    public   WhereClauseBuilder<EasySqlBuilder> Where()                       => new(in this, ref this);
    internal WhereClauseBuilder<TNext>          Where<TNext>( in TNext next ) => new(in next, ref this);


    public OrderByClauseBuilder Order()  => new(ref this);
    public GroupByClauseBuilder Group()  => new(ref this);
    public InsertClauseBuilder  Insert() => new(ref this);
    public UpdateClauseBuilder  Update() => new(ref this);
    public DeleteClauseBuilder  Delete() => new(ref this);
    public JoinClauseBuilder    Join()   => new(ref this);
}