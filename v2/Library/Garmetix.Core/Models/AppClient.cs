
/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/*
 * AppClient.cs
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

using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Models
{
    public class AppClient : CEntity
    {

        [Display(Name = "Name")]public string? Name { get; set; }
        [Display(Name = "Company")]public Guid CompanyId { get; set; }
        [Display(Name = "Client Secret")]public string? ClientSecret { get; set; }
        [Display(Name = "Device ID")]public string? DeviceId { get; set; }
        [Display(Name = "Device Type")]public string? DeviceType { get; set; }
        [Display(Name = "Device Token")]public string? DeviceToken { get; set; }
        [Display(Name = "IP Address")]public string? IpAddress { get; set; }
    }
}