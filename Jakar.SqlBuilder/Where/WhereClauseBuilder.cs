namespace Jakar.SqlBuilder;


public struct WhereClauseBuilder<TNext>( in TNext next, ref EasySqlBuilder builder )
{
    private readonly TNext          __next    = next;
    private          EasySqlBuilder __builder = builder;


    public TNext Exists()
    {
        __builder.Add(EXISTS);
        return __next;
    }


    // public WhereChainBuilder<TNext> Chain( string condition )
    // {
    //     _builder.Add(KeyWords.EXISTS, condition);
    //     return new WhereChainBuilder<TNext>(this, ref _builder);
    // }
    // public WhereChainBuilder<TNext> Chain<TValue>( string columnName, TValue _ ) => Chain<TValue>(columnName);
    // public WhereChainBuilder<TNext> Chain<TValue>( string columnName )
    // {
    //     _builder.Add(KeyWords.EXISTS, columnName.GetName<TValue>());
    //     return new WhereChainBuilder<TNext>(this, ref _builder);
    // }


    public WhereClauseBuilder<TNext> Between( string columnName )
    {
        __builder.Add(BETWEEN, columnName);
        return this;
    }


    public WhereClauseBuilder<TNext> And()
    {
        __builder.Add(AND);
        return this;
    }
    public WhereClauseBuilder<TNext> Or()
    {
        __builder.Add(OR);
        return this;
    }


    public WhereClauseBuilder<TNext> IsNull( string columnName )
    {
        __builder.Add(IS, NULL, columnName);
        return this;
    }
    public WhereClauseBuilder<TNext> IsNotNull( string columnName )
    {
        __builder.Add(IS, NOT, NULL, columnName);
        return this;
    }


    public WhereClauseBuilder<TNext> Like( string pattern )
    {
        __builder.Add(LIKE, pattern);
        return this;
    }


    public WhereInChainBuilder<TNext> In( string columnName )
    {
        __builder.Add(columnName, IN);
        return new WhereInChainBuilder<TNext>(this, ref __builder);
    }
    public WhereInChainBuilder<TNext> NotIn( string columnName )
    {
        __builder.Add(columnName, NOT, IN)
                 .Begin();

        return new WhereInChainBuilder<TNext>(this, ref __builder);
    }


    public SelectClauseBuilder<WhereClauseBuilder<TNext>>       Select()       => new(this, ref __builder);
    public AggregateFunctionsBuilder<WhereClauseBuilder<TNext>> WithFunction() => new(this, ref __builder);


    public TNext Done()
    {
        __builder.VerifyParentheses();
        return __next;
    }
}
