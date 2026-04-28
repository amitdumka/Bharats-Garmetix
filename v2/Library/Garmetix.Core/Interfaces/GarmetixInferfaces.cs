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
}
 
