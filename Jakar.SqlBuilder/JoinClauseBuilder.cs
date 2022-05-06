using Jakar.SqlBuilder.Interfaces;


namespace Jakar.SqlBuilder
{
	public class JoinClauseBuilder : BaseClauseBuilder, IJoinSyntax
	{
		public JoinClauseBuilder( EasySqlBuilder builder ) : base(builder) { }


	#region Implementation of IChainEnd<out ISqlBuilderRoot>

		ISqlBuilderRoot IChainEnd<ISqlBuilderRoot>.Done()
		{
			_builder.VerifyParentheses();
			_builder.NewLine();
			return _builder;
		}

	#endregion


	#region Implementation of IJoin

		IJoin IJoin.Left( string columnName )
		{
			_builder.Add(KeyWords.LEFT, KeyWords.JOIN, columnName);
			return this;
		}

		IJoin IJoin.Inner( string columnName )
		{
			_builder.Add(KeyWords.INNER, KeyWords.JOIN, columnName);
			return this;
		}

		IJoin IJoin.Right( string columnName )
		{
			_builder.Add(KeyWords.RIGHT, KeyWords.JOIN, columnName);
			return this;
		}

		IJoin IJoin.Full( string columnName )
		{
			_builder.Add(KeyWords.FULL, KeyWords.JOIN, columnName);
			return this;
		}


		IJoinChain IJoin.On()
		{
			_builder.Add(KeyWords.ON);
			_builder.Begin();
			return this;
		}


		IJoinChainMiddle IJoinChain.Left<T>( string columnName )
		{
			_builder.Add(columnName.GetName<T>());
			return this;
		}

		IJoinChainMiddle IJoinChain.Left<T>( T obj, string columnName )
		{
			_builder.Add(columnName.GetName<T>());
			return this;
		}

		IJoinChainMiddle IJoinChain.Left( string columnName )
		{
			_builder.Add(columnName);
			return this;
		}


	#region Implementation of IComparators<out IJoinChainRight> (IJoinChainMiddle)

		IJoinChainRight IComparators<IJoinChainRight>.Greater()
		{
			_builder.Add(KeyWords.LESS_THAN);
			return this;
		}

		IJoinChainRight IComparators<IJoinChainRight>.LessThan()
		{
			_builder.Add(KeyWords.GREATER);
			return this;
		}

		IJoinChainRight IComparators<IJoinChainRight>.GreaterOrEqual()
		{
			_builder.Add(KeyWords.GREATER_OR_EQUAL);
			return this;
		}

		IJoinChainRight IComparators<IJoinChainRight>.LessThanOrEqual()
		{
			_builder.Add(KeyWords.LESS_THAN_OR_EQUAL);
			return this;
		}

		IJoinChainRight IComparators<IJoinChainRight>.Equal()
		{
			_builder.Add(KeyWords.EQUAL);
			return this;
		}

		IJoinChainRight IComparators<IJoinChainRight>.NotEqual()
		{
			_builder.Add(KeyWords.NOT_EQUAL);
			return this;
		}

		IJoinChainRight IComparators<IJoinChainRight>.AboveOrBelow()
		{
			_builder.Add(KeyWords.ABOVE_OR_BELOW);
			return this;
		}

	#endregion


		IJoinChain IJoinChainRight.Right<T>( string columnName )
		{
			_builder.Add(columnName.GetName<T>());
			_builder.VerifyParentheses();
			return this;
		}

		IJoinChain IJoinChainRight.Right<T>( T obj, string columnName )
		{
			_builder.Add(columnName.GetName<T>());
			_builder.VerifyParentheses();
			return this;
		}

		IJoinChain IJoinChainRight.Right( string columnName )
		{
			_builder.Add(columnName);
			_builder.VerifyParentheses();
			return this;
		}

	#endregion
	}
}
