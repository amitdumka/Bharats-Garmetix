/*
 * CurrentSession.cs
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

using Garmetix.Core.Enums;

namespace Garmetix.Core.Sessions
{
    public class CurrentSession
    {
        public bool IsAutoLoginEnabled { get; set; }
        public Guid? CompanyId { get; set; }
        public string? UserName { get; set; }
        public string? CompanyDatabaseFileName { get; set; }
        public DateTime LoginTime { get; set; }
        public LoginRole UserRole { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
    }
}