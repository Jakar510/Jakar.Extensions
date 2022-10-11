#nullable enable
namespace Jakar.SqlBuilder;


public struct JoinChainBuilderLeft
{
    private readonly JoinClauseBuilder _join;
    private          EasySqlBuilder    _builder;
    public JoinChainBuilderLeft( in JoinClauseBuilder join, ref EasySqlBuilder builder )
    {
        _join    = join;
        _builder = builder;
    }


    public JoinChainBuilderMiddle Left<T>( string columnName )
    {
        _builder.Add( columnName.GetName<T>() );
        return new JoinChainBuilderMiddle( _join, ref _builder );
    }
    public JoinChainBuilderMiddle Left<T>( T obj, string columnName )
    {
        _builder.Add( columnName.GetName<T>() );
        return new JoinChainBuilderMiddle( _join, ref _builder );
    }
    public JoinChainBuilderMiddle Left( string columnName )
    {
        _builder.Add( columnName );
        return new JoinChainBuilderMiddle( _join, ref _builder );
    }
}
