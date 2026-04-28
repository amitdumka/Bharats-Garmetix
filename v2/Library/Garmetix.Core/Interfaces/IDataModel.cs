/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

 
using System.Linq.Expressions;
using Garmetix.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for data access operations on entities of type T, supporting querying, creation, update,
    /// deletion, and retrieval with optional filtering and paging capabilities.
    /// </summary>
    /// <remarks>Implementations of this interface provide asynchronous and synchronous methods for managing
    /// entities, including support for filtering, ordering, and pagination. Methods may return null or empty
    /// collections if no matching entities are found. Thread safety and transaction management depend on the specific
    /// implementation.</remarks>
    /// <version>1.0.0</version>
    /// <typeparam name="T">The type of entity managed by the data model. Must implement the IEntity interface.</typeparam>
    public interface IDataModel<T> where T : class, IEntity
    {
        int Count();

        string GetError();
        //TODO: Order is from EFCore so implement that
        IQueryable<T> Where<TKey>(Expression<Func<T, bool>> predict, Order? orderby, Expression<Func<T, TKey>> order);

        Task<T?> SaveAsync(T entity, bool isNew = true);

        Task<List<T>?> SaveAllAsync(List<T> values, bool isNew = true);

        Task<T?> GetByIdAsync(Guid id);
        Task<T?> CreateAsync(T entity);

        Task<T?> UpdateAsync(T entity);

        Task<T?> GetAsync(Guid id);

        Task<bool> DeleteAsync(Guid id);

        Task<List<T>?> GetAllAsync();

        Task<List<T>?> GetAllAsync(int pageNumber, int pageSize);

        Task<List<T>?> GetAllAsync(Expression<Func<T, bool>>? filter = null);

        Task<bool> IsExists(Guid id);

        Task<int> GetCountAsync(Expression<Func<T, bool>>? filter = null);

        int GetCount(Expression<Func<T, bool>>? filter = null);

        Task<List<T>?> GetAllWithoutLinkAsync();

        Task<List<T>?> GetAllWithoutLinkAsync(int pageNumber, int pageSize);
    }
}
 
