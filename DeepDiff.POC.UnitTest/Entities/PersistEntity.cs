namespace DeepDiff.POC.UnitTest.Entities;

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
