﻿namespace OneLine.Models
{
    public interface IPaged<T>
    {
        /// <summary>
        /// The page index
        /// </summary>
        int PageIndex { get; set; }
        /// <summary>
        /// The page size
        /// </summary>

        int PageSize { get; set; }
        /// <summary>
        /// The total count of records
        /// </summary>

        int TotalCount { get; set; }
        /// <summary>
        /// The total pages
        /// </summary>

        int TotalPages { get; set; }
        /// <summary>
        /// Determines wether you can go back to the previous page of records
        /// </summary>

        bool HasPreviousPage { get; set; }
        /// <summary>
        /// Determines wether you can go foward to the next page of record
        /// </summary>

        bool HasNextPage { get; set; }
        /// <summary>
        /// The data
        /// </summary>

        T Data { get; set; }
    }
}
