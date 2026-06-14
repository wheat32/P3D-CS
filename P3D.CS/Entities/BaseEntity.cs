namespace P3D;

// TODO Phase 12: full BaseEntity port
public abstract class BaseEntity
{
    public enum EntityTypes
    {
        XmlEntity,
        Entity
    }

    private readonly EntityTypes _entityType;

    protected BaseEntity(EntityTypes entityType = EntityTypes.Entity)
    {
        _entityType = entityType;
    }
}
