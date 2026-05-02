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
 * Salesman.cs
 * 
 * This file contains the definitions of classes related to invoicing in the Garmetix application.
 * It includes classes for Salesman, BaseInvoice, Customer, and Vendor.
 * These classes are designed to represent the various entities and their relationships within the invoicing system.
 * 
 * The classes are structured to support features such as tracking invoice details, calculating tax amounts, and managing customer and vendor information.
 * They also include properties for handling payment modes, GST system, and VAT system.
 * 
 * The use of attributes like [JsonIgnore] helps to control the serialization of certain properties when converting objects to JSON format.
 * This is particularly useful for properties that are calculated or derived from other properties, ensuring that only relevant data is included in API responses or data storage.
 *
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

namespace Garmetix.Core.Models.Inventory
{
    public class Salesman : StoreBase
    {
        [Display(Name = "Name")] public string Name { get; set; } = "Manager";
        [Display(Name = "Employee", AutoGenerateField = false)] public Guid? EmployeeId { get; set; }
        [Display(Name = "Active")] public bool Active { get; set; } = true;
    }
}