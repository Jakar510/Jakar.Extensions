namespace Jakar.SqlBuilder;


public struct DeleteClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder _builder = builder;


    public DeleteChainBuilder From( string tableName, string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { _builder.Add( FROM, tableName ); }

        else { _builder.Add( FROM, tableName, AS, alias ); }

        _builder.NewLine();

        return new DeleteChainBuilder( this, ref _builder );
    }

    public DeleteChainBuilder From<T>( T _, string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { _builder.Add( FROM, typeof(T).GetTableName() ); }

        else { _builder.Add( FROM, typeof(T).GetName(), AS, alias ); }

        _builder.NewLine();

        return new DeleteChainBuilder( this, ref _builder );
    }

    public DeleteChainBuilder From<T>( string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { _builder.Add( FROM, typeof(T).GetTableName() ); }

        else { _builder.Add( FROM, typeof(T).GetName(), AS, alias ); }

        _builder.NewLine();

        return new DeleteChainBuilder( this, ref _builder );
    }


    public WhereClauseBuilder<DeleteClauseBuilder> Where() => _builder.Add( WHERE ).Begin().Where( in this );

    public EasySqlBuilder Done()
    {
        _builder.VerifyParentheses();
        return _builder;
    }


    public DeleteChainBuilder All()
    {
        _builder.Add( DELETE );
        return new DeleteChainBuilder( this, ref _builder );
    }

    public EasySqlBuilder Column( string columnName )
    {
        _builder.Add( DELETE, columnName );
        return _builder;
    }
}
