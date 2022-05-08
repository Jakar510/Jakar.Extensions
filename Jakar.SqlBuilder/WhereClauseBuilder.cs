namespace Jakar.SqlBuilder;


public class WhereClauseBuilder : BaseClauseBuilder, IWhereSyntax
{
    public WhereClauseBuilder( EasySqlBuilder builder ) : base(builder) { }


    #region Implementation of IWhere

    ISqlBuilderRoot IWhere.Filter<T>( string columnName, T obj )
    {
        _builder.Add(KeyWords.FILTER);
        return _builder;
    }

    ISqlBuilderRoot IWhere.Filter( string condition )
    {
        _builder.Add(KeyWords.FILTER);
        return _builder;
    }

    ISqlBuilderRoot IWhere.Exists()
    {
        _builder.Add(KeyWords.EXISTS);
        return _builder;
    }

    IWhereChain IWhere.Chain<T>( string columnName, T target )
    {
        _builder.Add(KeyWords.EXISTS, columnName.GetName<T>());
        return this;
    }

    IWhereChain IWhere.Chain( string condition )
    {
        _builder.Add(KeyWords.EXISTS, condition);
        return this;
    }

    IWhereChain IWhere.Between( string columnName )
    {
        _builder.Add(KeyWords.BETWEEN, columnName);
        return this;
    }

    IWhere IWhere.IsNull( string columnName )
    {
        _builder.Add(KeyWords.IS, KeyWords.NULL, columnName);
        return this;
    }

    IWhere IWhere.IsNotNull( string columnName )
    {
        _builder.Add(KeyWords.IS, KeyWords.NOT, KeyWords.NULL, columnName);
        return this;
    }

    IWhere IWhere.Like( string pattern )
    {
        _builder.Add(KeyWords.LIKE, pattern);
        return this;
    }


    IWhereInChain IWhere.In( string columnName )
    {
        _builder.Add(columnName, KeyWords.IN);
        _builder.Begin();
        return this;
    }

    ISqlBuilderRoot IWhere.In( string columnName, params string[] conditions )
    {
        _builder.Add(columnName, KeyWords.IN);
        _builder.Begin();
        _builder.AddRange(',', conditions);
        return _builder.End();
    }

    IWhereInChain IWhere.NotIn( string columnName )
    {
        _builder.Add(columnName, KeyWords.NOT, KeyWords.IN);
        _builder.Begin();
        return this;
    }

    ISqlBuilderRoot IWhere.NotIn( string columnName, params string[] conditions )
    {
        _builder.Add(columnName, KeyWords.NOT, KeyWords.IN);
        _builder.Begin();
        _builder.AddRange(',', conditions);
        return _builder.End();
    }

    #endregion


    #region Implementation of IWhereInChain

    IWhereInChain IWhereInChain.Next()
    {
        _builder.Comma();
        return this;
    }

    ISqlBuilderRoot IWhereInChain.Done()
    {
        _builder.VerifyParentheses();
        _builder.NewLine();
        return _builder;
    }

    ISelectorLoop<IWhereInChain> IWhereInChain.Select()
    {
        _builder.Begin();
        return this;
    }

    // ISelectorLoop<IWhereInChain> IWhereInChain.Select( string columnName ) { return this; }

    #endregion


    #region Implementation of IChainEnd<out ISqlBuilderRoot>

    ISqlBuilderRoot IChainEnd<ISqlBuilderRoot>.Done()
    {
        _builder.VerifyParentheses();
        _builder.NewLine();
        return _builder;
    }

    #endregion


    #region Implementation of INextChain<out IWhere>

    IWhere INextChain<IWhere>.Next() { return this; }

    #endregion


    #region Implementation of IWhereChain

    IWhereChain IWhereChain.And<T>( T   obj, string columnName ) { return this; }
    IWhereChain IWhereChain.And( string condition ) { return this; }
    IWhereChain IWhereChain.Or<T>( T    obj, string columnName ) { return this; }
    IWhereChain IWhereChain.Or( string  condition ) { return this; }
    IWhereChain IWhereChain.Not<T>( T   obj, string columnName ) { return this; }
    IWhereChain IWhereChain.Not( string condition ) { return this; }

    #endregion


    #region Implementation of IFromSyntax<out IWhereInChain>

    IWhereInChain IFromSyntax<IWhereInChain>.From( string     tableName, string? alias ) { return this; }
    IWhereInChain IFromSyntax<IWhereInChain>.From<T>( T       obj,       string? alias ) { return this; }
    IWhereInChain IFromSyntax<IWhereInChain>.From<T>( string? alias ) { return this; }

    #endregion


    #region Implementation of IAggregateFunctions<out ISelectorLoop<IWhereInChain>>

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.All()
    {
        _builder.AggregateFunction();
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Distinct()
    {
        _builder.AggregateFunction(KeyWords.DISTINCT);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Average()
    {
        _builder.AggregateFunction(KeyWords.AVERAGE);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Average( string columnName )
    {
        _builder.AggregateFunction(KeyWords.AVERAGE, columnName);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Average<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.AVERAGE, columnName.GetName<T>());
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Average<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.AVERAGE, obj.GetName(columnName));
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Sum()
    {
        _builder.AggregateFunction(KeyWords.SUM);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Sum( string columnName )
    {
        _builder.AggregateFunction(KeyWords.SUM, columnName);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Sum<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.SUM, columnName.GetName<T>());
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Sum<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.SUM, obj.GetName(columnName));
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Count()
    {
        _builder.AggregateFunction(KeyWords.COUNT);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Count( string columnName )
    {
        _builder.AggregateFunction(KeyWords.COUNT, columnName);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Count<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.COUNT, columnName.GetName<T>());
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Count<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.COUNT, obj.GetName(columnName));
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Min()
    {
        _builder.AggregateFunction(KeyWords.MIN);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Min( string columnName )
    {
        _builder.AggregateFunction(KeyWords.MIN, columnName);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Min<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.MIN, columnName.GetName<T>());
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Min<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.MIN, obj.GetName(columnName));
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Max()
    {
        _builder.AggregateFunction(KeyWords.MAX);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Max( string columnName )
    {
        _builder.AggregateFunction(KeyWords.MAX, columnName);
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Max<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.MAX, columnName.GetName<T>());
        return this;
    }

    ISelectorLoop<IWhereInChain> IAggregateFunctions<ISelectorLoop<IWhereInChain>>.Max<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.MAX, obj.GetName(columnName));
        return this;
    }

    #endregion
}