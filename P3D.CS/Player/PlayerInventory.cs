namespace P3D;

public class InventorySlot
{
    public Items.Item? Item { get; set; }
    public int Amount { get; set; }
}

// TODO Phase 3: full PlayerInventory port
public class PlayerInventory
{
    public bool HasRunningShoes;
    private List<InventorySlot> _slots = [];

    public int Count => _slots.Count;
    public InventorySlot this[int index] => _slots[index];

    public void Clear() { }
    public void AddItem(String itemID, int amount) { }
    public void RemoveItem(String itemID, int amount) { }
    public int GetItemAmount(String itemID) => 0;
    public String GetMessageReceive(Items.Item item, int amount) => "";
}
