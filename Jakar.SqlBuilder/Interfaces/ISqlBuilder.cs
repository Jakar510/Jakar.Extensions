#nullable enable
namespace Jakar.SqlBuilder.Interfaces;


// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IWhereSyntax : IWhere, IWhereInChain, IWhereChain, ISelectorLoop<IWhereInChain> { }



public interface IOrderBySyntax : IOrderBy, IOrderByChain { }



public interface IGroupBySyntax : IGroupBy, IGroupByChain { }



public interface IInsertSyntax : IInsertInto, IDataInsert { }



public interface IUpdateSyntax : IUpdate, IUpdateChain { }



public interface IDeleteSyntax : IDelete, IDeleteChain { }



public interface IJoinSyntax : IJoin, IJoinChain, IJoinChainMiddle, IJoinChainRight { }



public interface IHavingSyntax : IHaving { }



public interface ISelectorSyntax : ISelector { }



public interface ISqlBuilderRoot
{
    public string Result { get; }
    public ISelector Select();
    public ISelector Union();
    public ISelector UnionAll();
    public IWhere Where();
    public IOrderBy Order();
    public IGroupBy Group();
    public IInsertInto Insert();
    public IUpdate Update();
    public IDelete Delete();
    public IJoin Join();
}
