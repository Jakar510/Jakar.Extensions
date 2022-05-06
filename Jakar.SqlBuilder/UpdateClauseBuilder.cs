using System.Collections.Generic;
using Jakar.Extensions.General;
using Jakar.SqlBuilder.Interfaces;


namespace Jakar.SqlBuilder
{
	public class UpdateClauseBuilder : BaseClauseBuilder, IUpdateSyntax
	{
		protected readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
		public UpdateClauseBuilder( EasySqlBuilder builder ) : base(builder) { }


	#region Implementation of IToSyntax<out IUpdateChain>

		IUpdateChain IToSyntax<IUpdateChain>.To( string tableName )
		{
			_builder.Add(KeyWords.UPDATE, tableName, KeyWords.SET);
			return this;
		}

		IUpdateChain IToSyntax<IUpdateChain>.To<T>( T obj )
		{
			_builder.Add(KeyWords.UPDATE, typeof(T).GetName(), KeyWords.SET);
			return this;
		}

		IUpdateChain IToSyntax<IUpdateChain>.To<T>()
		{
			_builder.Add(KeyWords.UPDATE, typeof(T).GetName(), KeyWords.SET);
			return this;
		}

	#endregion


	#region Implementation of IChainEnd<out ISqlBuilderRoot>

		ISqlBuilderRoot IChainEnd<ISqlBuilderRoot>.Done()
		{
			foreach ( ( string? columnName, string value ) in _cache )
			{
				_builder.Add(columnName, "=", value + ',');
			}

			_builder.NewLine();
			return _builder;
		}

	#endregion


	#region Implementation of IUpdateChain

		IUpdateChain IUpdateChain.Set<T>( string columnName, T obj )
		{
			_cache[columnName] = obj?.ToString() ?? KeyWords.NULL;
			return this;
		}

		IWhere IUpdateChain.Where()
		{
			( (IUpdateChain) this ).Done();

			return _builder.Where();
		}

	#endregion
	}
}
