namespace NPS.Core.Entities;

public class BaseEntity
{
    public Guid id;
    public DateTime createdAt;
    public DateTime createdBy;
    public DateTime updatedAt;
    public DateTime updatedBy;

    public BaseEntity()
    {
        createdAt = DateTime.Now;
        updatedAt = DateTime.Now;
    }
}
