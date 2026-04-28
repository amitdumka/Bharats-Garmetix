/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/* 
 * IPageModel.cs
 * 
 * Interface for page models that represent a page with a list of entities. This interface defines the properties and commands that are common to all page models that represent a list of entities.
 * 
 * * This file is part of Garmetix.
 * 
 * Garmetix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Garmetix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Garmetix.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Garmetix.Base.PageModels
{
    /// <summary>
    ///  Interface for page models that represent a page with a list of entities. This interface defines the properties and commands that are common to all page models that represent a list of entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IPageModel<TEntity>
    {
        string Title { get; }
        bool IsBusy { get; }
        bool IsDataLoading { get; }
        bool EnableAdd { get; }
        ObservableCollection<TEntity> Entities { get; }
        ICommand LoadInitialDataCommand { get; }
        ICommand LoadMoreDataCommand { get; }
        ICommand AddButtonCommand { get; }
    }
}