namespace Jakar.SqlBuilder;


public struct UpdateClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder _builder = builder;


    public UpdateChainBuilder To( string tableName )
    {
        _builder.Add( UPDATE, tableName, SET );
        return new UpdateChainBuilder( this, ref _builder );
    }
    public UpdateChainBuilder To<T>( T _ )
    {
        _builder.Add( UPDATE, typeof(T).GetName(), SET );
        return new UpdateChainBuilder( this, ref _builder );
    }
    public UpdateChainBuilder To<T>()
    {
        _builder.Add( UPDATE, typeof(T).GetName(), SET );
        return new UpdateChainBuilder( this, ref _builder );
    }


    public EasySqlBuilder Done() => _builder.NewLine();


    public WhereClauseBuilder<EasySqlBuilder> Where()
    {
        Done();
        return _builder.Where();
    }
}
