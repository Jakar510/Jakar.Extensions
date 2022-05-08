using Jakar.Extensions.General;



namespace Jakar.SqlBuilder;


public class DeleteClauseBuilder : BaseClauseBuilder, IDeleteSyntax
{
    public DeleteClauseBuilder( EasySqlBuilder builder ) : base(builder) { }


    #region Implementation of IFromSyntax<out IDeleteChain>

    IDeleteChain IFromSyntax<IDeleteChain>.From( string tableName, string? alias )
    {
        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, tableName); }

        else { _builder.Add(KeyWords.FROM, tableName, KeyWords.AS, alias); }

        _builder.NewLine();

        return this;
    }

    IDeleteChain IFromSyntax<IDeleteChain>.From<T>( T obj, string? alias )
    {
        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, typeof(T).GetTableName()); }

        else { _builder.Add(KeyWords.FROM, typeof(T).GetName(), KeyWords.AS, alias); }

        _builder.NewLine();

        return this;
    }

    IDeleteChain IFromSyntax<IDeleteChain>.From<T>( string? alias )
    {
        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, typeof(T).GetTableName()); }

        else { _builder.Add(KeyWords.FROM, typeof(T).GetName(), KeyWords.AS, alias); }

        _builder.NewLine();

        return this;
    }

    #endregion


    #region Implementation of IDeleteChain

    IWhere IDeleteChain.Where()
    {
        _builder.Add(KeyWords.WHERE);
        _builder.Begin();
        return _builder.Where();
    }

    ISqlBuilderRoot IChainEnd<ISqlBuilderRoot>.Done()
    {
        _builder.VerifyParentheses();
        return _builder;
    }

    #endregion


    #region Implementation of IDelete

    IDeleteChain IDelete.All()
    {
        _builder.Add(KeyWords.DELETE);
        return this;
    }

    ISqlBuilderRoot IDelete.Column( string columnName )
    {
        _builder.Add(KeyWords.DELETE, columnName);
        return _builder;
    }

    #endregion
}