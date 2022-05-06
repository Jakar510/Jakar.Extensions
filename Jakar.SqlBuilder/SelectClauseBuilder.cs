using System;
using Jakar.Extensions.General;
using Jakar.SqlBuilder.Interfaces;


namespace Jakar.SqlBuilder
{
	public class SelectClauseBuilder : BaseClauseBuilder, ISelectorSyntax
	{
		public SelectClauseBuilder( EasySqlBuilder builder ) : base(builder) { }


	#region Implementation of IFromSyntax<out ISqlBuilderRoot>

		ISqlBuilderRoot IFromSyntax<ISqlBuilderRoot>.From( string tableName, string? alias )
		{
			if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM); }

			else { _builder.Add(KeyWords.FROM, tableName, KeyWords.AS, alias); }

			return _builder.NewLine();
		}

		ISqlBuilderRoot IFromSyntax<ISqlBuilderRoot>.From<T>( T obj, string? alias )
		{
			if ( obj is null ) { throw new NullReferenceException(nameof(obj)); }

			if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, obj.GetTableName()); }

			else { _builder.Add(KeyWords.FROM, obj.GetName(), KeyWords.AS, alias); }

			return _builder.NewLine();
		}

		ISqlBuilderRoot IFromSyntax<ISqlBuilderRoot>.From<T>( string? alias )
		{
			if ( string.IsNullOrWhiteSpace(alias) ) { _builder.Add(KeyWords.FROM, typeof(T).GetTableName()); }

			else { _builder.Add(KeyWords.FROM, typeof(T).GetName(), KeyWords.AS, alias); }

			return _builder.NewLine();
		}

	#endregion


	#region Implementation of IAggregateFunctions<out ISelector>

		ISelector IAggregateFunctions<ISelector>.All()
		{
			_builder.Star();
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Distinct()
		{
			_builder.Add(KeyWords.DISTINCT);
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Average()
		{
			_builder.Add(KeyWords.AVERAGE + "(*)");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Average( string columnName )
		{
			_builder.Add(KeyWords.AVERAGE, "(", columnName, ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Average<T>( string columnName )
		{
			_builder.Add(KeyWords.AVERAGE, "(", columnName.GetName<T>(), ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Average<T>( T obj, string columnName )
		{
			_builder.Add(KeyWords.AVERAGE, "(", obj.GetName(columnName), ")");
			return this;
		}


		ISelector IAggregateFunctions<ISelector>.Sum()
		{
			_builder.Add(KeyWords.SUM + "(*)");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Sum( string columnName )
		{
			_builder.Add(KeyWords.SUM, "(", columnName, ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Sum<T>( string columnName )
		{
			_builder.Add(KeyWords.SUM, "(", columnName.GetName<T>(), ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Sum<T>( T obj, string columnName )
		{
			_builder.Add(KeyWords.SUM, "(", obj.GetName(columnName), ")");
			return this;
		}


		ISelector IAggregateFunctions<ISelector>.Count()
		{
			_builder.Add(KeyWords.COUNT + "(*)");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Count( string columnName )
		{
			_builder.Add(KeyWords.COUNT, "(", columnName, ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Count<T>( string columnName )
		{
			_builder.Add(KeyWords.COUNT, "(", columnName.GetName<T>(), ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Count<T>( T obj, string columnName )
		{
			_builder.Add(KeyWords.COUNT, "(", obj.GetName(columnName), ")");
			return this;
		}


		ISelector IAggregateFunctions<ISelector>.Min()
		{
			_builder.Add(KeyWords.MIN + "(*)");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Min( string columnName )
		{
			_builder.Add(KeyWords.MIN, "(", columnName, ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Min<T>( string columnName )
		{
			_builder.Add(KeyWords.MIN, "(", columnName.GetName<T>(), ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Min<T>( T obj, string columnName )
		{
			_builder.Add(KeyWords.MIN, "(", obj.GetName(columnName), ")");
			return this;
		}


		ISelector IAggregateFunctions<ISelector>.Max()
		{
			_builder.Add(KeyWords.MAX + "(*)");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Max( string columnName )
		{
			_builder.Add(KeyWords.MAX, "(", columnName, ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Max<T>( string columnName )
		{
			_builder.Add(KeyWords.MAX, "(", columnName.GetName<T>(), ")");
			return this;
		}

		ISelector IAggregateFunctions<ISelector>.Max<T>( T obj, string columnName )
		{
			_builder.Add(KeyWords.MAX, "(", obj.GetName(columnName), ")");
			return this;
		}

	#endregion


	#region Implementation of ISelector

		ISelector ISelector.Next( string columnName )
		{
			_builder.Add(columnName + ',');
			return this;
		}

		ISelector ISelector.Next<T>( string columnName )
		{
			_builder.Add(columnName.GetName<T>() + ',');
			return this;
		}

		ISelector ISelector.Next( string alias, string separator, params string[] columnNames )
		{
			_builder.Begin();
			_builder.AddRange(separator, columnNames);
			_builder.End();
			_builder.Add(KeyWords.AS, alias);

			return this;
		}

		ISelector ISelector.Next<T>( string alias, string separator, params string[] columnNames )
		{
			_builder.Begin();
			_builder.AddRange<T>(separator, columnNames);
			_builder.End();
			_builder.Add(KeyWords.AS, alias);

			return this;
		}

	#endregion
	}
}
