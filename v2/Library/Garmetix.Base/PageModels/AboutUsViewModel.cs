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
 * AboutUsViewModel.cs
 * 
 * A ViewModel for the About Us page of the Garmetix application, providing information about the app and its developers.
 * 
 * This file is part of Garmetix.
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
using CommunityToolkit.Mvvm.ComponentModel;

namespace Garmetix.Base.PageModels
{
    /// <summary>
    /// ViewModel for the About Us page, providing information about the application and its developers.
    /// </summary>
    public partial class AboutUsViewModel : ObservableObject
    {
        [ObservableProperty] private string appName = "Garmetix";
        [ObservableProperty] private string version = "Version 5.0";
        [ObservableProperty] private string developer = "Amit Kumar";
        [ObservableProperty] private string clientName = "Aadwika Fashion";
    }
}
