﻿namespace Jakar.SqlBuilder;


public struct JoinClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder _builder = builder;


    public EasySqlBuilder Done() => _builder.VerifyParentheses().NewLine();


    public JoinClauseBuilder Left( string columnName )
    {
        _builder.Add( KeyWords.LEFT, KeyWords.JOIN, columnName );
        return this;
    }
    public JoinClauseBuilder Inner( string columnName )
    {
        _builder.Add( KeyWords.INNER, KeyWords.JOIN, columnName );
        return this;
    }
    public JoinClauseBuilder Right( string columnName )
    {
        _builder.Add( KeyWords.RIGHT, KeyWords.JOIN, columnName );
        return this;
    }
    public JoinClauseBuilder Full( string columnName )
    {
        _builder.Add( KeyWords.FULL, KeyWords.JOIN, columnName );
        return this;
    }


    public JoinChainBuilderLeft On()
    {
        _builder.Add( KeyWords.ON ).Begin();

        return new JoinChainBuilderLeft( this, ref _builder );
    }
}
