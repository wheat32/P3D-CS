using System.Reflection;

namespace P3D.Items;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class ItemAttribute : Attribute
{
    public int Id { get; }
    public String Name { get; }

    public ItemAttribute(int id, String name)
    {
        Id = id;
        Name = name;
    }
}

internal struct ItemIdentifier
{
    public String Name;
    public int Id;
}
