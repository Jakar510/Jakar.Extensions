namespace Jakar.SqlBuilder;


public struct SelectClauseBuilder
{
    private EasySqlBuilder _builder;
    public SelectClauseBuilder( EasySqlBuilder builder ) => _builder = builder;


    public EasySqlBuilder From( string tableName, string? alias )
    {
        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM); }

        else { _builder.Add(KeyWords.FROM, tableName, KeyWords.AS, alias); }

        return _builder.NewLine();
    }
    public EasySqlBuilder From<T>( T obj, string? alias )
    {
        if ( obj is null ) { throw new NullReferenceException(nameof(obj)); }

        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, obj.GetTableName()); }

        else { _builder.Add(KeyWords.FROM, obj.GetName(), KeyWords.AS, alias); }

        return _builder.NewLine();
    }
    public EasySqlBuilder From<T>( string? alias )
    {
        if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, typeof(T).GetTableName()); }

        else { _builder.Add(KeyWords.FROM, typeof(T).GetName(), KeyWords.AS, alias); }

        return _builder.NewLine();
    }


    public SelectClauseBuilder Next( string columnName )
    {
        _builder.Add(columnName);
        return this;
    }
    public SelectClauseBuilder Next<T>( string columnName )
    {
        _builder.Add(columnName.GetName<T>());
        return this;
    }
    public SelectClauseBuilder Next( params string[] columnNames ) => Next(',', columnNames);
    public SelectClauseBuilder Next( char separator, params string[] columnNames )
    {
        _builder.Begin().AddRange(separator, columnNames).End();
        return this;
    }


    public SelectClauseBuilder Next<T>( params string[] columnNames ) => Next<T>(',', columnNames);
    public SelectClauseBuilder Next<T>( char separator, params string[] columnNames )
    {
        _builder.Begin().AddRange<T>(separator, columnNames).End();
        return this;
    }


    public SelectClauseBuilder NextAs( string alias, params string[] columnNames ) => NextAs(alias, ',', columnNames);
    public SelectClauseBuilder NextAs( string alias, char separator, params string[] columnNames )
    {
        _builder.Begin().AddRange(separator, columnNames).End().Add(KeyWords.AS, alias);
        return this;
    }

    public SelectClauseBuilder NextAs<T>( string alias, params string[] columnNames ) => NextAs<T>(alias, ',', columnNames);
    public SelectClauseBuilder NextAs<T>( string alias, char separator, params string[] columnNames )
    {
        _builder.Begin().AddRange<T>(separator, columnNames).End().Add(KeyWords.AS, alias);
        return this;
    }
}
