namespace TestApp.Entities;

public abstract class IdEntity : PersistEntity
{
    public Guid Id { get; set; }

    protected IdEntity()
    {
        Id = Guid.NewGuid();
    }
}
