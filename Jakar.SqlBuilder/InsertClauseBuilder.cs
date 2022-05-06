using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jakar.SqlBuilder.Interfaces;


namespace Jakar.SqlBuilder
{
	public class InsertClauseBuilder : BaseClauseBuilder, IInsertSyntax
	{
		protected readonly Dictionary<string, string> _cache     = new Dictionary<string, string>();
		protected readonly List<string>               _itemCache = new List<string>();

		public InsertClauseBuilder( EasySqlBuilder builder ) : base(builder) { }

		~InsertClauseBuilder()
		{
			_itemCache.Clear();
			_cache.Clear();
		}


	#region Inserts with Reflection

		ISqlBuilderRoot IInsertInto.Into<T>( string tableName, T obj )
		{
			_builder.Add(KeyWords.INSERT, KeyWords.INTO, obj.GetName(tableName));
			return SetValues(obj);
		}

		ISqlBuilderRoot IInsertInto.Into<T>( T obj )
		{
			_builder.Add(KeyWords.INSERT, KeyWords.INTO, typeof(T).GetName());
			return SetValues(obj);
		}

		ISqlBuilderRoot IInsertInto.Into<T>( IEnumerable<T> obj )
		{
			_builder.Add(KeyWords.INSERT, KeyWords.INTO, typeof(T).GetName());
			return SetValues(obj);
		}

		protected ISqlBuilderRoot SetValues<T>( T obj )
		{
			foreach ( PropertyInfo info in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty) )
			{
				_cache[info.Name] = info.GetValue(obj)?.ToString() ?? KeyWords.NULL;
			}


			_builder.Begin();
			_builder.AddRange(',', _cache.Keys);
			_builder.End();

			_builder.Add(KeyWords.VALUES);

			_builder.Begin();
			_builder.AddRange(',', _cache.Values);
			_builder.End();


			return _builder.NewLine();
		}

	#endregion


	#region Implementation of IInsertInto

		IDataInsert IInsertInto.In( string tableName )
		{
			_builder.Add(KeyWords.INSERT, KeyWords.INTO, tableName);
			return this;
		}

		IDataInsert IInsertInto.In<T>()
		{
			_builder.Add(KeyWords.INSERT, KeyWords.INTO, typeof(T).GetName());
			return this;
		}

		IDataInsert IInsertInto.In<T>( T obj )
		{
			_builder.Add(KeyWords.INSERT, KeyWords.INTO, typeof(T).GetName());
			return this;
		}

	#endregion


	#region Implementation of IChainEnd<out ISqlBuilderRoot>

		ISqlBuilderRoot IChainEnd<ISqlBuilderRoot>.Done()
		{
			if ( _itemCache.Any() && _cache.Any() ) { throw new FormatException($"Cannot use both {nameof(IDataInsert)}.{nameof(IDataInsert.With)} methods together."); }

			if ( _itemCache.Any() )
			{
				_builder.Add(KeyWords.VALUES);

				_builder.Begin();
				_builder.AddRange(',', _itemCache);
				_builder.End();
			}

			if ( _cache.Any() )
			{
				_builder.Begin();
				_builder.AddRange(',', _cache.Keys);
				_builder.End();

				_builder.Add(KeyWords.VALUES);

				_builder.Begin();
				_builder.AddRange(',', _cache.Values);
				_builder.End();
			}


			return _builder.NewLine();
		}

		IDataInsert IDataInsert.With<T>( string columnName, T data )
		{
			_cache[columnName] = data?.ToString() ?? KeyWords.NULL;
			return this;
		}

		IDataInsert IDataInsert.With<T>( T data )
		{
			_itemCache.Add(data?.ToString() ?? KeyWords.NULL);
			return this;
		}

	#endregion
	}
}
