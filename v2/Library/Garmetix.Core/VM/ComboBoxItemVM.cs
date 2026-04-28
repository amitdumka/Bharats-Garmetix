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
 * ComboBoxItemVM.cs
 * 
 * This file defines the ComboBoxItemVM class, which represents an item in a combo box.
 * Each item has a unique identifier (Id) and a display name (Name).
 * The Id is generated automatically when a new instance of ComboBoxItemVM is created.
 */
namespace Garmetix.Core.VM
{
    public class ComboBoxItemVM
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }
}
