/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Garmetix.Core.Interfaces
{
    /// <summary>
    /// Defines an entity with a unique identifier.
    /// <version>2.0.0</version>
    /// </summary>
    public interface IEntity { [Key][Display(AutoGenerateField = false)] Guid Id { get; set; } }

    /// <summary>
    /// Defines a contract for displaying user notifications, such as success, error, warning, and confirmation dialogs,
    /// in an asynchronous manner.
    /// </summary>
    /// <version>2.0.0</version>
    /// <remarks>Implementations of this interface are responsible for presenting notifications to the user
    /// and may vary in how dialogs are displayed depending on the application platform. All methods are asynchronous
    /// and intended to be called from UI or service code that needs to inform or prompt the user.</remarks>
    public interface INotificationService
    {
        Task ShowSuccessAsync(string message);
        Task ShowErrorAsync(string title, string exceptionMessage);
        Task ShowWarningAsync(string message);
        Task<bool> ShowConfirmAsync(string title, string message);
    }

    /// <summary>
    /// Defines a generic contract for a repository that provides asynchronous and synchronous data access,
    /// manipulation, and transaction operations for entities derived from BaseModel.
    /// </summary>
    /// <remarks>This interface abstracts common data access patterns, including querying, creation, updating,
    /// deletion, and transactional operations. It is intended to be implemented by classes that interact with a data
    /// store, such as a database or an in-memory collection. Implementations should ensure thread safety and proper
    /// resource management where applicable.</remarks>
    /// <version>2.0.0</version>
    /// <typeparam name="T">The type of entity managed by the repository. Must inherit from BaseModel.</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {

        //Query Methods
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(Guid id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        // Create  and Update Methods
        Task<T?> CreateAsync(T entity);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);

        //Delete Methods
        Task DeleteAsync(T entity);
        Task HardDeleteAsync(T entity);

        //Transaction Method
        Task ExecuteTransactionAsync(Func<Task> action);

        //Our custom methods 
        Task<T?> SaveAsync(T entity, bool isNew = true);
        Task<List<T>?> SaveAllAsync(List<T> values, bool isNew = true);
        IQueryable<T> Where<TKey>(Expression<Func<T, bool>> predict, Order? orderby, Expression<Func<T, TKey>> order);
        Task<List<T>?> GetAllAsync(int pageNumber, int pageSize);

        Task<List<T>?> GetAllAsync(Expression<Func<T, bool>>? filter = null);

        Task<bool> IsExists(Guid id);

        Task<int> GetCountAsync(Expression<Func<T, bool>>? filter = null);

        int GetCount(Expression<Func<T, bool>>? filter = null);

        Task<List<T>?> GetAllWithoutLinkAsync();

        Task<List<T>?> GetAllWithoutLinkAsync(int pageNumber, int pageSize);

        int Count(Expression<Func<T, bool>>? filter = null);
        int Count();
        string GetError();
    }

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
 
