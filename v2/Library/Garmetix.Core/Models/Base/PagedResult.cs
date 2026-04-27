/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

namespace Garmetix.Core.Models.Base
{
    /// <summary>
    /// PagedResult is a generic class that represents a paginated result set. It contains information about the total number of items, total pages, current page, page size, and the list of items for the current page. This class is commonly used in APIs to return paginated data to clients.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }= 0;
        public int TotalPages { get; set; }= 0;
        public int Page { get; set; }= 0;
        public int PageSize { get; set; }= 0;
        public List<T> Items { get; set; } = [];
    }



}