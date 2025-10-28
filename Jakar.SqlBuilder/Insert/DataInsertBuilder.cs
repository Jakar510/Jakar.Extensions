// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  3:22 PM

namespace Jakar.SqlBuilder;


public struct DataInsertBuilder( in InsertClauseBuilder insert, ref EasySqlBuilder builder )
{
    private readonly Dictionary<string, string> __cache   = new();
    private readonly InsertClauseBuilder        __insert  = insert;
    private          EasySqlBuilder             __builder = builder;


    public DataInsertBuilder With<TValue>( string columnName, TValue data )
    {
        __cache[columnName] = data?.ToString() ?? NULL;
        return this;
    }


    public EasySqlBuilder Done()
    {
        if ( !__cache.Any() ) { return __builder.NewLine(); }

        __builder.Begin();
        __builder.AddRange(',', __cache.Keys);
        __builder.End();

        __builder.Add(VALUES);

        __builder.Begin();
        __builder.AddRange(',', __cache.Values);
        __builder.End();


        return __builder.NewLine();
    }
}
