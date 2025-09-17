namespace Jakar.SqlBuilder;


public struct DeleteClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder __builder = builder;


    public DeleteChainBuilder From( string tableName, string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { __builder.Add( FROM, tableName ); }

        else { __builder.Add( FROM, tableName, AS, alias ); }

        __builder.NewLine();

        return new DeleteChainBuilder( this, ref __builder );
    }

    public DeleteChainBuilder From<TValue>( TValue _, string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { __builder.Add( FROM, typeof(TValue).GetTableName() ); }

        else { __builder.Add( FROM, typeof(TValue).GetName(), AS, alias ); }

        __builder.NewLine();

        return new DeleteChainBuilder( this, ref __builder );
    }

    public DeleteChainBuilder From<TValue>( string? alias )
    {
        if ( string.IsNullOrWhiteSpace( alias ) ) { __builder.Add( FROM, typeof(TValue).GetTableName() ); }

        else { __builder.Add( FROM, typeof(TValue).GetName(), AS, alias ); }

        __builder.NewLine();

        return new DeleteChainBuilder( this, ref __builder );
    }


    public WhereClauseBuilder<DeleteClauseBuilder> Where() => __builder.Add( WHERE ).Begin().Where( in this );

    public EasySqlBuilder Done()
    {
        __builder.VerifyParentheses();
        return __builder;
    }


    public DeleteChainBuilder All()
    {
        __builder.Add( DELETE );
        return new DeleteChainBuilder( this, ref __builder );
    }

    public EasySqlBuilder Column( string columnName )
    {
        __builder.Add( DELETE, columnName );
        return __builder;
    }
}
