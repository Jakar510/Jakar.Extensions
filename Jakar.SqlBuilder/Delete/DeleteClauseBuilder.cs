namespace Jakar.SqlBuilder;


public struct DeleteClauseBuilder
{
    private EasySqlBuilder _builder;
    public DeleteClauseBuilder( ref EasySqlBuilder builder ) => _builder = builder;


    public DeleteChainBuilder From( string tableName, string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { _builder.Add( KeyWords.FROM, tableName ); }

        else { _builder.Add( KeyWords.FROM, tableName, KeyWords.AS, alias ); }

        _builder.NewLine();

        return new DeleteChainBuilder( this, ref _builder );
    }

    public DeleteChainBuilder From<T>( T _, string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { _builder.Add( KeyWords.FROM, typeof(T).GetTableName() ); }

        else { _builder.Add( KeyWords.FROM, typeof(T).GetName(), KeyWords.AS, alias ); }

        _builder.NewLine();

        return new DeleteChainBuilder( this, ref _builder );
    }

    public DeleteChainBuilder From<T>( string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { _builder.Add( KeyWords.FROM, typeof(T).GetTableName() ); }

        else { _builder.Add( KeyWords.FROM, typeof(T).GetName(), KeyWords.AS, alias ); }

        _builder.NewLine();

        return new DeleteChainBuilder( this, ref _builder );
    }


    public WhereClauseBuilder<DeleteClauseBuilder> Where() => _builder.Add( KeyWords.WHERE ).Begin().Where( in this );

    public EasySqlBuilder Done()
    {
        _builder.VerifyParentheses();
        return _builder;
    }


    public DeleteChainBuilder All()
    {
        _builder.Add( KeyWords.DELETE );
        return new DeleteChainBuilder( this, ref _builder );
    }

    public EasySqlBuilder Column( string columnName )
    {
        _builder.Add( KeyWords.DELETE, columnName );
        return _builder;
    }
}
