namespace Jakar.SqlBuilder;


public struct UpdateClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder __builder = builder;


    public UpdateChainBuilder To( string tableName )
    {
        __builder.Add(UPDATE, tableName, SET);
        return new UpdateChainBuilder(this, ref __builder);
    }
    public UpdateChainBuilder To<TValue>( TValue _ )
    {
        __builder.Add(UPDATE, typeof(TValue).GetName(), SET);
        return new UpdateChainBuilder(this, ref __builder);
    }
    public UpdateChainBuilder To<TValue>()
    {
        __builder.Add(UPDATE, typeof(TValue).GetName(), SET);
        return new UpdateChainBuilder(this, ref __builder);
    }


    public EasySqlBuilder Done() => __builder.NewLine();


    public WhereClauseBuilder<EasySqlBuilder> Where()
    {
        Done();
        return __builder.Where();
    }
}
