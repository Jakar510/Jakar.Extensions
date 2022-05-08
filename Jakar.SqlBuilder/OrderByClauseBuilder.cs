namespace Jakar.SqlBuilder;


public class OrderByClauseBuilder : BaseClauseBuilder, IOrderBySyntax
{
    public OrderByClauseBuilder( EasySqlBuilder builder ) : base(builder) { }


    #region Implementation of IOrderBy

    ISqlBuilderRoot IOrderBy.By( string separator, params string[] columnNames )
    {
        _builder.Begin();
        _builder.Add(KeyWords.ORDER, KeyWords.BY);
        _builder.AddRange(separator, columnNames);
        _builder.End();
        return _builder;
    }

    IOrderByChain IOrderBy.Chain()
    {
        _builder.Add(KeyWords.ORDER, KeyWords.BY);
        _builder.Begin();
        return this;
    }

    IOrderByChain IOrderBy.Chain( string columnName )
    {
        _builder.Add(KeyWords.ORDER, KeyWords.BY);
        _builder.Begin();
        _builder.Add(columnName);
        return this;
    }

    #endregion


    #region Implementation of IChainEnd<out ISqlBuilderRoot>

    ISqlBuilderRoot IChainEnd<ISqlBuilderRoot>.Done()
    {
        _builder.VerifyParentheses();
        _builder.NewLine();
        return _builder;
    }

    #endregion


    #region Implementation of INextChain<out IOrderBy>

    IOrderBy INextChain<IOrderBy>.Next()
    {
        _builder.VerifyParentheses();
        _builder.Add(KeyWords.ORDER, KeyWords.BY);
        _builder.Begin();
        return this;
    }

    #endregion


    #region Implementation of ISorters<out IOrderByChain>

    IOrderByChain ISorters<IOrderByChain>.Ascending()
    {
        _builder.Add(KeyWords.ASC + ',');
        return this;
    }

    IOrderByChain ISorters<IOrderByChain>.Ascending( string columnName )
    {
        _builder.Add(columnName, KeyWords.ASC + ',');
        return this;
    }

    IOrderByChain ISorters<IOrderByChain>.Descending()
    {
        _builder.Add(KeyWords.DESC + ',');
        return this;
    }

    IOrderByChain ISorters<IOrderByChain>.Descending( string columnName )
    {
        _builder.Add(columnName, KeyWords.DESC + ',');
        return this;
    }

    IOrderByChain IOrderByChain.By( string columnName )
    {
        _builder.Add(columnName);
        return this;
    }

    #endregion


    #region Implementation of IAggregateFunctions<out IOrderByChain>

    IOrderByChain IAggregateFunctions<IOrderByChain>.All()
    {
        _builder.AggregateFunction();
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Distinct()
    {
        _builder.AggregateFunction(KeyWords.DISTINCT);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Average()
    {
        _builder.AggregateFunction(KeyWords.AVERAGE);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Average( string columnName )
    {
        _builder.AggregateFunction(KeyWords.AVERAGE, columnName);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Average<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.AVERAGE, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Average<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.AVERAGE, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Sum()
    {
        _builder.AggregateFunction(KeyWords.SUM);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Sum( string columnName )
    {
        _builder.AggregateFunction(KeyWords.SUM, columnName);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Sum<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.SUM, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Sum<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.SUM, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Count()
    {
        _builder.AggregateFunction(KeyWords.COUNT);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Count( string columnName )
    {
        _builder.AggregateFunction(KeyWords.COUNT, columnName);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Count<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.COUNT, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Count<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.COUNT, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Min()
    {
        _builder.AggregateFunction(KeyWords.MIN);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Min( string columnName )
    {
        _builder.AggregateFunction(KeyWords.MIN, columnName);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Min<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.MIN, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Min<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.MIN, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Max()
    {
        _builder.AggregateFunction(KeyWords.MAX);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Max( string columnName )
    {
        _builder.AggregateFunction(KeyWords.MAX, columnName);
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Max<T>( string columnName )
    {
        _builder.AggregateFunction(KeyWords.MAX, columnName.GetName<T>());
        return this;
    }

    IOrderByChain IAggregateFunctions<IOrderByChain>.Max<T>( T obj, string columnName )
    {
        _builder.AggregateFunction(KeyWords.MAX, columnName.GetName<T>());
        return this;
    }

    #endregion
}