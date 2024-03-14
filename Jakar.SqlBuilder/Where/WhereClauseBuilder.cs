namespace Jakar.SqlBuilder;


public struct WhereClauseBuilder<TNext>( in TNext next, ref EasySqlBuilder builder )
{
    private readonly TNext          _next    = next;
    private          EasySqlBuilder _builder = builder;


    public TNext Exists()
    {
        _builder.Add( EXISTS );
        return _next;
    }


    // public WhereChainBuilder<TNext> Chain( string condition )
    // {
    //     _builder.Add(KeyWords.EXISTS, condition);
    //     return new WhereChainBuilder<TNext>(this, ref _builder);
    // }
    // public WhereChainBuilder<TNext> Chain<T>( string columnName, T _ ) => Chain<T>(columnName);
    // public WhereChainBuilder<TNext> Chain<T>( string columnName )
    // {
    //     _builder.Add(KeyWords.EXISTS, columnName.GetName<T>());
    //     return new WhereChainBuilder<TNext>(this, ref _builder);
    // }


    public WhereClauseBuilder<TNext> Between( string columnName )
    {
        _builder.Add( BETWEEN, columnName );
        return this;
    }


    public WhereClauseBuilder<TNext> And()
    {
        _builder.Add( AND );
        return this;
    }
    public WhereClauseBuilder<TNext> Or()
    {
        _builder.Add( OR );
        return this;
    }


    public WhereClauseBuilder<TNext> IsNull( string columnName )
    {
        _builder.Add( IS, NULL, columnName );
        return this;
    }
    public WhereClauseBuilder<TNext> IsNotNull( string columnName )
    {
        _builder.Add( IS, NOT, NULL, columnName );
        return this;
    }


    public WhereClauseBuilder<TNext> Like( string pattern )
    {
        _builder.Add( LIKE, pattern );
        return this;
    }


    public WhereInChainBuilder<TNext> In( string columnName )
    {
        _builder.Add( columnName, IN );
        return new WhereInChainBuilder<TNext>( this, ref _builder );
    }
    public WhereInChainBuilder<TNext> NotIn( string columnName )
    {
        _builder.Add( columnName, NOT, IN ).Begin();

        return new WhereInChainBuilder<TNext>( this, ref _builder );
    }


    public SelectClauseBuilder<WhereClauseBuilder<TNext>>       Select()       => new(this, ref _builder);
    public AggregateFunctionsBuilder<WhereClauseBuilder<TNext>> WithFunction() => new(this, ref _builder);


    public TNext Done()
    {
        _builder.VerifyParentheses();
        return _next;
    }
}
