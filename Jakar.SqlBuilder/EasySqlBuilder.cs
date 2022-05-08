namespace Jakar.SqlBuilder;


public sealed class EasySqlBuilder : ISqlBuilderRoot
{
    private StringBuilder? _cache;
    private bool           _needToClose;

    internal StringBuilder Cache  => _cache ?? throw new ObjectDisposedException($"{nameof(EasySqlBuilder)} can only be used once per instance.", new NullReferenceException(nameof(_cache)));
    public   string        Result => ToString();


    public EasySqlBuilder() : this(1024) { }
    public EasySqlBuilder( int capacity ) => _cache = new StringBuilder(capacity);


    internal void AddRange<T>( char separator, IEnumerable<string> names ) where T : class => AddRange(separator, names.Select(KeyWords.GetName<T>));

    internal EasySqlBuilder AddRange( char separator, IEnumerable<string> names )
    {
        Cache.AppendJoin(separator, names);

        return Space();
    }

    internal void AddRange<T>( string separator, IEnumerable<string> names ) where T : class => AddRange(separator, names.Select(KeyWords.GetName<T>));

    internal EasySqlBuilder AddRange( string separator, IEnumerable<string> names )
    {
        Cache.AppendJoin(separator, names);
        return Space();
    }


    internal EasySqlBuilder Add( params string[] names ) => AddRange(' ', names);

    internal EasySqlBuilder Add( char c )
    {
        Cache.Append(c);
        return this;
    }


    internal EasySqlBuilder NewLine()
    {
        Cache.AppendLine();
        return this;
    }


    internal EasySqlBuilder AggregateFunction( char func = '*' )
    {
        Cache.Append(func);
        return Space();
    }

    internal EasySqlBuilder AggregateFunction( string func, string? columnName = default )
    {
        Cache.Append(func);
        Cache.Append('(');

        if ( columnName is null ) { Cache.Append('*'); }
        else { Cache.Append(columnName); }

        Cache.Append(')');

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
        var result = Cache.Append(';').ToString();


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


    public ISelector Select() => new SelectClauseBuilder(this);

    public ISelector Union()
    {
        Add("UNION");
        return new SelectClauseBuilder(this);
    }

    public ISelector UnionAll()
    {
        Add("UNION ALL");
        return new SelectClauseBuilder(this);
    }

    public IWhere Where() => new WhereClauseBuilder(this);
    public IOrderBy Order() => new OrderByClauseBuilder(this);
    public IGroupBy Group() => new GroupByClauseBuilder(this);
    public IInsertInto Insert() => new InsertClauseBuilder(this);
    public IUpdate Update() => new UpdateClauseBuilder(this);
    public IDelete Delete() => new DeleteClauseBuilder(this);
    public IJoin Join() => new JoinClauseBuilder(this);

    
}
