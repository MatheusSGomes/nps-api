namespace NPS.Core.Entities;

public class BaseEntity
{
    public Guid id { get; private set; }
    public DateTime createdAt { get; private set; }
    public DateTime createdBy { get; private set; }
    public DateTime updatedAt { get; private set; }
    public DateTime updatedBy { get; private set; }

    public BaseEntity()
    {
        createdAt = DateTime.Now;
        updatedAt = DateTime.Now;
    }
}
