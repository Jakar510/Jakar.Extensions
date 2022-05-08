namespace Jakar.SqlBuilder;


public struct UpdateClauseBuilder
{
    private EasySqlBuilder _builder;
    public UpdateClauseBuilder( ref EasySqlBuilder builder ) => _builder = builder;


    public UpdateChainBuilder To( string tableName )
    {
        _builder.Add(KeyWords.UPDATE, tableName, KeyWords.SET);
        return new UpdateChainBuilder(this, ref _builder);
    }
    public UpdateChainBuilder To<T>( T _ )
    {
        _builder.Add(KeyWords.UPDATE, typeof(T).GetName(), KeyWords.SET);
        return new UpdateChainBuilder(this, ref _builder);
    }
    public UpdateChainBuilder To<T>()
    {
        _builder.Add(KeyWords.UPDATE, typeof(T).GetName(), KeyWords.SET);
        return new UpdateChainBuilder(this, ref _builder);
    }


    public EasySqlBuilder Done() => _builder.NewLine();


    public WhereClauseBuilder<EasySqlBuilder> Where()
    {
        Done();
        return _builder.Where();
    }
}
