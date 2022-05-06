﻿using System;
using Jakar.Extensions.General;


namespace Jakar.SqlBuilder.Interfaces
{
	public interface IFromSyntax<out TNext>
	{
		/// <summary>
		/// Uses the passed <paramref name="tableName"/>
		/// </summary>
		/// <returns><typeparamref name="TNext"/></returns>
		public TNext From( string tableName, string? alias = default );

		/// <summary>
		/// <para>
		/// <para>
		/// Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(object)"/>
		/// </para>
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type being passed</typeparam>
		/// <returns><typeparamref name="TNext"/></returns>
		public TNext From<T>( T obj, string? alias = default );

		/// <summary>
		/// <para>
		/// Uses the <see cref="Type"/> of <typeparamref name="T"/> to get the table_name using <see cref="DapperTableExtensions.GetTableName(Type)"/>
		/// </para>
		/// </summary>
		/// <typeparam name="T">The type being passed</typeparam>
		/// <returns><typeparamref name="TNext"/></returns>
		public TNext From<T>( string? alias = default ) where T : class;
	}
}
