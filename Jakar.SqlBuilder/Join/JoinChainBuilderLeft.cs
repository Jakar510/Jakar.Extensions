namespace Jakar.SqlBuilder;


public struct JoinChainBuilderLeft( in JoinClauseBuilder join, ref EasySqlBuilder builder )
{
    private readonly JoinClauseBuilder _join    = join;
    private          EasySqlBuilder    _builder = builder;


    public JoinChainBuilderMiddle Left<TValue>( string columnName )
    {
        _builder.Add( columnName.GetName<TValue>() );
        return new JoinChainBuilderMiddle( _join, ref _builder );
    }
    public JoinChainBuilderMiddle Left<TValue>( TValue obj, string columnName )
    {
        _builder.Add( columnName.GetName<TValue>() );
        return new JoinChainBuilderMiddle( _join, ref _builder );
    }
    public JoinChainBuilderMiddle Left( string columnName )
    {
        _builder.Add( columnName );
        return new JoinChainBuilderMiddle( _join, ref _builder );
    }
}
