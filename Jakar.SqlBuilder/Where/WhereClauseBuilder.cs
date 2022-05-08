namespace Jakar.SqlBuilder;


public struct WhereClauseBuilder<TNext>
{
    private readonly TNext          _next;
    private          EasySqlBuilder _builder;
    public WhereClauseBuilder( in TNext next, ref EasySqlBuilder builder )
    {
        _next    = next;
        _builder = builder;
    }


    public TNext Filter<T>( string columnName, T _ )
    {
        _builder.Add(KeyWords.FILTER);
        return _next;
    }
    public TNext Filter( string condition )
    {
        _builder.Add(KeyWords.FILTER).Space().Add(condition);
        return _next;
    }

    public TNext Exists()
    {
        _builder.Add(KeyWords.EXISTS);
        return _next;
    }


    public WhereChainBuilder<TNext> Chain( string condition )
    {
        _builder.Add(KeyWords.EXISTS, condition);
        return new WhereChainBuilder<TNext>(this, ref _builder);
    }
    public WhereChainBuilder<TNext> Chain<T>( string columnName, T _ ) => Chain<T>(columnName);
    public WhereChainBuilder<TNext> Chain<T>( string columnName )
    {
        _builder.Add(KeyWords.EXISTS, columnName.GetName<T>());
        return new WhereChainBuilder<TNext>(this, ref _builder);
    }


    public WhereChainBuilder<TNext> Between( string columnName )
    {
        _builder.Add(KeyWords.BETWEEN, columnName);
        return new WhereChainBuilder<TNext>(this, ref _builder);
    }

    public WhereClauseBuilder<TNext> IsNull( string columnName )
    {
        _builder.Add(KeyWords.IS, KeyWords.NULL, columnName);
        return this;
    }
    public WhereClauseBuilder<TNext> IsNotNull( string columnName )
    {
        _builder.Add(KeyWords.IS, KeyWords.NOT, KeyWords.NULL, columnName);
        return this;
    }

    public WhereClauseBuilder<TNext> Like( string pattern )
    {
        _builder.Add(KeyWords.LIKE, pattern);
        return this;
    }


    public WhereInChainBuilder<TNext> In( string columnName )
    {
        _builder.Add(columnName, KeyWords.IN);
        return new WhereInChainBuilder<TNext>(this, ref _builder);
    }
    public TNext In( string columnName, params string[] conditions )
    {
        _builder.Add(columnName, KeyWords.IN).Begin().AddRange(',', conditions).End();
        return _next;
    }

    public TNext NotIn( string columnName )
    {
        _builder.Add(columnName, KeyWords.NOT, KeyWords.IN).Begin();
        return _next;
    }
    public TNext NotIn( string columnName, params string[] conditions )
    {
        _builder.Add(columnName, KeyWords.NOT, KeyWords.IN).Begin().AddRange(',', conditions).End();
        return _next;
    }


    public WhereConditionBuilder<WhereClauseBuilder<TNext>> Select() => new(in this, ref _builder);
    public AggregateFunctionsBuilder<WhereClauseBuilder<TNext>> WithFunction() => new(in this, ref _builder);


    public WhereClauseBuilder<TNext> Next()
    {
        _builder.Comma();
        return this;
    }
}
