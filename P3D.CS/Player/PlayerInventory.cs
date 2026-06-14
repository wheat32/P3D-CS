using System.Collections;

namespace P3D;

public class PlayerInventory : IEnumerable<PlayerInventory.ItemContainer>
{
    public class ItemContainer
    {
        public String ItemID { get; set; } = String.Empty;
        public int Amount { get; set; }
    }

    public bool HasRunningShoes;
    private List<ItemContainer> _slots = [];

    public int Count => _slots.Count;
    public ItemContainer this[int index] => _slots[index];

    public IEnumerator<ItemContainer> GetEnumerator() => _slots.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _slots.GetEnumerator();

    public void Clear() => _slots.Clear();

    public void AddItem(String itemID, int amount)
    {
        ItemContainer? existing = _slots.Find(s => s.ItemID.Equals(itemID));
        if (existing != null)
        {
            existing.Amount += amount;
        }
        else
        {
            _slots.Add(new ItemContainer { ItemID = itemID, Amount = amount });
        }
    }

    public void RemoveItem(String itemID, int amount)
    {
        ItemContainer? existing = _slots.Find(s => s.ItemID.Equals(itemID));
        if (existing == null) return;
        existing.Amount -= amount;
        if (existing.Amount <= 0)
        {
            _slots.Remove(existing);
        }
    }

    public int GetItemAmount(String itemID)
    {
        ItemContainer? existing = _slots.Find(s => s.ItemID.Equals(itemID));
        return existing?.Amount ?? 0;
    }

    public String GetMessageReceive(Items.Item item, int amount) => String.Empty;
}
