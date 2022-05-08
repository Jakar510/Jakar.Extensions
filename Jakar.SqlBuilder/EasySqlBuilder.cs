namespace Jakar.SqlBuilder;


public struct EasySqlBuilder
{
    private readonly StringBuilder _sb;
    private          bool          _needToClose = false;

    public string Result => ToString();


    public EasySqlBuilder() : this(1024) { }
    public EasySqlBuilder( int capacity ) => _sb = new StringBuilder(capacity);


    internal EasySqlBuilder AddRange<T>( char separator, IEnumerable<string> names ) => AddRange(separator, names.Select(KeyWords.GetName<T>));
    internal EasySqlBuilder AddRange( char separator, IEnumerable<string> names )
    {
        _sb.AppendJoin(separator, names);

        return Space();
    }
    internal EasySqlBuilder AddRange<T>( string separator, IEnumerable<string> names ) => AddRange(separator, names.Select(KeyWords.GetName<T>));
    internal EasySqlBuilder AddRange( string separator, IEnumerable<string> names )
    {
        _sb.AppendJoin(separator, names);
        return Space();
    }


    internal EasySqlBuilder Add( char c )
    {
        _sb.Append(c);
        return this;
    }
    internal EasySqlBuilder Add( string value )
    {
        _sb.Append(value);
        return this;
    }
    internal EasySqlBuilder Add( params string[] names ) => AddRange(' ', names);


    internal EasySqlBuilder NewLine()
    {
        _sb.AppendLine();
        return this;
    }


    internal EasySqlBuilder AggregateFunction( char func = '*' )
    {
        _sb.Append(func);
        return Space();
    }
    internal EasySqlBuilder AggregateFunction( string func, string? columnName = default )
    {
        _sb.Append(func);
        Begin();

        if ( columnName is null ) { _sb.Append('*'); }
        else { _sb.Append(columnName); }

        End();

        return Space();
    }


    internal EasySqlBuilder Star() => Add('*');
    internal EasySqlBuilder Comma() => Add(',');
    internal EasySqlBuilder Space() => Add(' ');


    internal EasySqlBuilder Begin()
    {
        _needToClose = true;
        return Add('(');
    }
    internal EasySqlBuilder End()
    {
        _needToClose = false;
        return Add(')');
    }
    internal EasySqlBuilder VerifyParentheses()
    {
        if ( _needToClose ) { End(); }

        return this;
    }


    public override string ToString()
    {
        VerifyParentheses();
        var result = _sb.Append(';').ToString();


        // int start = result.LastIndexOf('(');
        // int end   = result.LastIndexOf(')');
        //
        // if ( start >= 0 )
        // {
        //     if ( end < start ) { throw new FormatException("Should not occur, no start ("); }
        // }
        // else if ( end >= 0 ) { throw new FormatException("Should not occur, no start ("); }

        if ( !result.IsBalanced() ) { throw new FormatException($@"String is not balanced! ""{result}"""); }

        return result;
    }


    public SelectClauseBuilder Select() => new(this);

    public SelectClauseBuilder Union()
    {
        Add("UNION");
        return new SelectClauseBuilder(this);
    }

    public SelectClauseBuilder UnionAll()
    {
        Add("UNION ALL");
        return new SelectClauseBuilder(this);
    }

    public WhereClauseBuilder<EasySqlBuilder> Where() => new(in this, ref  this);
    internal WhereClauseBuilder<TNext> Where<TNext>(in TNext next ) => new(in next, ref  this);
    public OrderByClauseBuilder Order() => new(ref this);
    public GroupByClauseBuilder Group() => new(ref this);
    public InsertClauseBuilder Insert() => new(ref this);
    public UpdateClauseBuilder Update() => new(ref this);
    public DeleteClauseBuilder Delete() => new(ref this);
    public JoinClauseBuilder Join() => new(ref this);
}
