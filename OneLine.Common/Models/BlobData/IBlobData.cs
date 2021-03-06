﻿using System;
using System.IO;

namespace OneLine.Models
{
    public interface IBlobData
    {
        /// <summary>
        /// Last modified
        /// </summary>
        DateTime LastModified { get; set; }
        /// <summary>
        /// The File Name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The File Input Name
        /// </summary>
        string InputName { get; set; }
        /// <summary>
        /// The file size
        /// </summary>
        long Size { get; set; }
        /// <summary>
        /// The file type
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// The file stream data
        /// </summary>
        Stream Data { get; set; }
    }
}
