using Bharat.ToolKits.Notifications;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Sessions;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Garmetix.Models.Bases;
using Garmetix.Models.Bharat.Enums;  
using Microsoft.EntityFrameworkCore;
using Sentry;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Garmetix.Core.DataModels
{
    public class DataModel<TEntity> : IDataModel<TEntity> where TEntity : class, IEntity
    {
        #region RoleBased

        protected static bool IsAdmin => SessionService.IsAdmin();
        protected static bool CanSave => SessionService.CanAddOrSaveRecords();
        protected static bool CanUpdate => SessionService.CanEditRecords();
        protected static bool CanDelete => SessionService.CanDeleteRecords();
        protected static bool CanView => SessionService.CanViewRecords();
        protected static bool CanViewAll => SessionService.CanViewReports();

        #endregion RoleBased

        #region ClientInfo

        public static Guid AppId = Guid.NewGuid();
        public static Guid ClientId => SessionService.CurrentSession.CompanyId.Value;
        public static Guid StoreGroupId => SessionService.CurrentSession.GroupId.Value;
        public static Guid StoreId => SessionService.CurrentSession.StoreId.Value;

        #endregion ClientInfo

        #region Fields

        protected DatabaseContext _localDb => DatabaseService.Instance.LocalDB;
        //protected readonly  DatabaseContext _localDb = DatabaseService.Instance.LocalDB;

        protected bool DTOEnabled { get; set; } = false;
        protected bool IsError { get; set; } = false;
        protected string? ErrorMessage { get; set; } = null;

        #endregion Fields

        public DatabaseContext GetContext()
        {
            return _localDb;
        }

        public int Count()
        {
            return _localDb.Set<TEntity>().Count();
        }

        public string GetError()
        {
            if (IsError)
            {
                return ErrorMessage ?? "Error not defined! Unknown error.";
            }
            return string.Empty;
        }

        public IQueryable<TEntity> Where<TKey>(Expression<Func<TEntity, bool>> predict, Order? orderby, Expression<Func<TEntity, TKey>> order)
        {
            if (order != null && orderby != null)
            {
                return orderby switch
                {
                    Order.Asc => GetContext().Set<TEntity>().AsNoTracking().Where(predict).OrderBy(order),
                    Order.Desc => GetContext().Set<TEntity>().AsNoTracking().Where(predict).OrderByDescending(order),
                    _ => GetContext().Set<TEntity>().AsNoTracking().Where(predict).OrderBy(order),
                };
            }

            return GetContext().Set<TEntity>().AsNoTracking().Where(predict);
        }

        public async Task<TEntity?> SaveAsync(TEntity entity, bool isNew = true)
        {
            //TODO: need to exception handling and error handling
            if (CanSave || CanUpdate)
            {
                try
                {
                    if (isNew)
                    {
                        await _localDb.AddAsync(entity);
                    }
                    else
                       if (!isNew && CanUpdate)
                    {
                        _localDb.Update(entity);
                    }
                    else
                    {
                        IsError = true;
                        ErrorMessage = "Access Denied!";
                        return null;
                    }

                    if (await _localDb.SaveChangesAsync() > 0)
                    {
                        //TODO: need to exception handling and error handling and Loggimng
                        return entity;
                    }
                    IsError = true;
                    ErrorMessage = $"Error Msg: {entity.GetType().Name} not saved!";
                }
                catch (Exception e)
                {
                    _localDb.Remove(entity);
                    IsError = true;
                    ErrorMessage = $"Error Msg: {entity.GetType().Name} not saved! Error :{e.Message} ";
                    Notify.LogError(e);
                    _ = SentrySdk.CaptureException(e);
                   // _ = NotificationService.AddNotification("Data Save Error", $"Error Msg: {entity.GetType().Name} not saved! Error :{e.Message} ");
                    return null;
                }
            }
            IsError = true;
            ErrorMessage = "Access Denied";
            return null;
        }

        public async Task<List<TEntity>?> SaveAllAsync(List<TEntity> values, bool isNew = true)
        {
            //TODO: need to add audit log
            //TODO: need to exception handling and error handling
            if (CanSave || CanUpdate)
            {
                if (isNew)
                {
                    await _localDb.AddRangeAsync(values);
                }
                else if (!isNew && CanUpdate)
                {
                    _localDb.UpdateRange(values);
                }
                else
                {
                    IsError = true;
                    ErrorMessage = "Access Denied!";
                    return null;
                }
                try
                {
                    if (await _localDb.SaveChangesAsync() > 0)
                    {
                        return values;
                    }
                    IsError = true;
                    ErrorMessage = $"Error Msg: {values.GetType().Name} not saved!";
                }
                catch (Exception e)
                {
                    IsError = true;
                    ErrorMessage = $"Error Msg:  {values.GetType().Name} not saved! Error : {e.Message} ";
                    return null;
                    throw;
                }
            }
            IsError = true;
            ErrorMessage = "Access Denied";
            return null;
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            if (CanView)
            {
                return await _localDb.FindAsync<TEntity>(id);
            }
            else
            {
                IsError = true;
                ErrorMessage = "Access Denied!";
                return null;
            }
        }

        public async Task<TEntity?> CreateAsync(TEntity entity)
        {
            if (CanSave && entity is not null)
            {
                await _localDb.AddAsync(entity);
                if (await _localDb.SaveChangesAsync() > 0)
                {
                    return entity;
                }
                else
                {
                    IsError = true;
                    ErrorMessage = $"Error Msg: {entity.GetType().Name} not saved!";
                    return null;
                }
            }
            else
            {
                IsError = true;
                ErrorMessage = "Access Denied!";
                return null;
            }
        }

        // Existing code...

        public async Task<TEntity?> UpdateAsync(TEntity entity)
        {
            if (CanUpdate && entity is not null)
            {
                if (_localDb.Set<TEntity>().Any(x => x.Id == entity.Id))
                {
                    _localDb.Update(entity);
                }
                else
                {
                    IsError = true;
                    ErrorMessage = $"Error Msg: {entity.GetType().Name} not saved!, {entity.Id} not found!";
                    return null;
                }
                if (await _localDb.SaveChangesAsync() > 0)
                {
                    return entity;
                }
                else
                {
                    IsError = true;
                    ErrorMessage = $"Error Msg: {entity.GetType().Name} not saved!";
                    return null;
                }
            }
            else
            {
                IsError = true;
                ErrorMessage = "Access Denied!";
                return null;
            }
        }

        public async Task<TEntity?> GetAsync(Guid id)
        {
            if (CanView)
            {
                return await _localDb.FindAsync<TEntity>(id);
            }
            else
            {
                IsError = true;
                ErrorMessage = "Access Denied!";
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            //TODO: need to add audit log
            //TODO: need to exception handling and error handling
            if (CanDelete)
            {
                var element = await _localDb.FindAsync<TEntity>(id);

                if (element != null)
                {
                    _localDb.Remove(entity: element);
                }
                else
                {
                    IsError = true;
                    ErrorMessage = $"Error Msg: {id} not found! of {typeof(TEntity).Name}";
                    return false;
                }
                if (await _localDb.SaveChangesAsync() > 0)
                {
                    return true;
                }
                IsError = true;
                ErrorMessage = $"Error Msg: {id} Not Deleted of {typeof(TEntity).Name}";
                return false;
            }
            IsError = true;
            ErrorMessage = "Access Denied";
            return false;
        }

        public virtual async Task<List<TEntity>?> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                if (CanViewAll)
                {
                    // Start with a queryable set of the entity with no tracking for performance.
                    var query = _localDb.Set<TEntity>().AsQueryable().AsNoTracking();

                    // Retrieve the entity metadata to inspect the navigation properties.
                    var entityType = _localDb.Model.FindEntityType(typeof(TEntity));
                    if (entityType != null)
                    {
                        // Loop through each navigation property and dynamically include it.
                        foreach (var navigation in entityType.GetNavigations())
                        {
                            query = query.Include(navigation.Name);
                        }
                    }

                    // Apply pagination directly on the query.
                    var pagedData = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

                    return pagedData;
                }
                else
                {
                    IsError = true;
                    ErrorMessage = "Not authorised to access!";
                    SentrySdk.CaptureMessage("Error: GetAll: Not authorised to access!");
                    return [];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                IsError = true;
                ErrorMessage = ex.Message;
                SentrySdk.ConfigureScope(scope => scope.SetExtra("Error", ex.Message));
                SentrySdk.CaptureMessage("Error: GetAll-Paged:\n" + ex.Message);
                SentrySdk.CaptureException(ex);

                return [];
            }
        }

        public virtual async Task<List<TEntity>?> GetAllWithoutLinkAsync(int pageNumber, int pageSize)
        {
            try
            {
                if (CanViewAll)
                {
                    var pagedData = await _localDb.Set<TEntity>().AsNoTracking().ToListAsync();
                    return pagedData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    IsError = true;
                    ErrorMessage = "Not Authozised to access!";
                    SentrySdk.CaptureMessage("Error: GetAll: Not Authozised to access!");
                    return [];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                IsError = true;
                ErrorMessage = ex.Message;
                SentrySdk.ConfigureScope(scope => scope.SetExtra("Error", ex.Message));
                SentrySdk.CaptureMessage("Error: GetAll-Paged:\n" + ex.Message);
                SentrySdk.CaptureException(ex);

                return [];
            }
        }

        public virtual async Task<List<TEntity>?> GetAllAsync()
        {
            try
            {
                if (CanViewAll)
                {
                    // return await _localDb.Set<TEntity>().AsNoTracking().ToListAsync();
                    var query = _localDb.Set<TEntity>().AsQueryable().AsNoTracking();

                    // Retrieve the entity metadata to inspect the navigation properties.
                    var entityType = _localDb.Model.FindEntityType(typeof(TEntity));
                    if (entityType != null)
                    {
                        // Loop through each navigation property and dynamically include it.
                        foreach (var navigation in entityType.GetNavigations())
                        {
                            if (navigation.Name != "Company" && navigation.Name != "Store" && navigation.Name != "Group")
                            {
                                query = query.Include(navigation.Name);
                            }
                        }
                    }
                    return await query.ToListAsync();
                }
                else
                {
                    IsError = true;
                    ErrorMessage = "Not Authozised to access!";
                    SentrySdk.CaptureMessage("Error: GetAll: Not Authozised to access!");
                    return [];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                IsError = true;
                ErrorMessage = ex.Message;
                SentrySdk.ConfigureScope(scope => scope.SetExtra("Error", ex.Message));
                SentrySdk.CaptureMessage("Error: GetAll:\n" + ex.Message);
                SentrySdk.CaptureException(ex);

                return [];
            }
        }

        public virtual async Task<List<TEntity>?> GetAllWithoutLinkAsync()
        {
            try
            {
                if (CanViewAll)
                {
                    return await _localDb.Set<TEntity>().AsNoTracking().ToListAsync();
                }
                else
                {
                    IsError = true;
                    ErrorMessage = "Not Authozised to access!";
                    SentrySdk.CaptureMessage("Error: GetAll: Not Authozised to access!");
                    return [];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                IsError = true;
                ErrorMessage = ex.Message;
                SentrySdk.ConfigureScope(scope => scope.SetExtra("Error", ex.Message));
                SentrySdk.CaptureMessage("Error: GetAll:\n" + ex.Message);
                SentrySdk.CaptureException(ex);

                return [];
            }
        }

        public virtual async Task<List<TEntity>?> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (CanViewAll)
            {
                return filter is null
                   ? await _localDb.Set<TEntity>().AsNoTracking().ToListAsync()
                   : await _localDb.Set<TEntity>().Where(filter).AsNoTracking().ToListAsync();
            }
            else
            {
                IsError = true;
                ErrorMessage = "Not Authozised to access!";
                return null;
            }
        }

        public async Task<bool> IsExists(Guid id)
        {
            return await _localDb.Set<TEntity>().AsNoTracking().AnyAsync(x => x.Id == id);
            //if (await _localDb.FindAsync<TEntity>(id) != null) return true; else return false;
        }

        /// <summary>
        /// Get The Count of Record
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (filter == null)
            {
                return await _localDb.Set<TEntity>().CountAsync();
            }
            else
            {
                return await _localDb.Set<TEntity>().CountAsync(filter);
            }
        }

        public int GetCount(Expression<Func<TEntity, bool>>? filter = null)
        {
            if (filter == null)
            {
                return _localDb.Set<TEntity>().Count();
            }
            else
            {
                return _localDb.Set<TEntity>().Count(filter);
            }
        }
    }
}