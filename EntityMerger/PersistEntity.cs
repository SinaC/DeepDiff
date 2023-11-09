﻿namespace EntityMerger
{
    public abstract class PersistEntity
    {
        public PersistChange PersistChange { get; set; }
    }

    public enum PersistChange
    {
        None,
        Insert,
        Update,
        Delete,
    }
}
