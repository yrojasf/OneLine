﻿namespace OneLine.Models
{
    public class Identifier<T> : IIdentifier<T>
        where T : class
    {
        public virtual T Model { get; set; }
    }
}
