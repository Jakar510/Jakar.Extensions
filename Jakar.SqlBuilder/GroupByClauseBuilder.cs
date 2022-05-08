namespace Jakar.SqlBuilder;


public class GroupByClauseBuilder : BaseClauseBuilder, IGroupBySyntax
{
    public GroupByClauseBuilder( EasySqlBuilder builder ) : base(builder) { }


    #region Implementation of IGroupBy

    ISqlBuilderRoot IGroupBy.By( string separator, params string[] columnNames )
    {
        _builder.Add(KeyWords.GROUP, KeyWords.BY, string.Join(separator, columnNames));
        return _builder;
    }

    IGroupByChain IGroupBy.Chain()
    {
        _builder.Add(KeyWords.GROUP, KeyWords.BY);
        return this;
    }

    IGroupByChain IGroupBy.Chain( string columnName )
    {
        _builder.Add(KeyWords.GROUP, KeyWords.BY, columnName);
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


    #region Implementation of INextChain<out IGroupBy>

    IGroupBy INextChain<IGroupBy>.Next()
    {
        _builder.VerifyParentheses();
        _builder.NewLine();
        return this;
    }

    #endregion


    #region Implementation of ISorters<out IGroupByChain>

    IGroupByChain ISorters<IGroupByChain>.Ascending()
    {
        _builder.Add(KeyWords.ASC);
        return this;
    }

    IGroupByChain ISorters<IGroupByChain>.Ascending( string columnName )
    {
        _builder.Add(columnName, KeyWords.ASC);
        return this;
    }

    IGroupByChain ISorters<IGroupByChain>.Descending()
    {
        _builder.Add(KeyWords.DESC);
        return this;
    }

    IGroupByChain ISorters<IGroupByChain>.Descending( string columnName )
    {
        _builder.Add(columnName, KeyWords.DESC);
        return this;
    }

    IGroupByChain IGroupByChain.And( string columnName )
    {
        _builder.Add(columnName + ',');
        return this;
    }

    #endregion
}