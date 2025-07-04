using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour,IInventory
{
    private ItemDataTable itemDataTable;
    public List<ItemSlot> inventorySlots = new List<ItemSlot>();
    public bool Initialized { get; private set; } = false;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => TableManager.Instance.loadComplete);
        itemDataTable = TableManager.Instance.GetTable<ItemDataTable>();
        Init();
        var item1 = itemDataTable.GetDataByID(6000);
        var item2 = itemDataTable.GetDataByID(6001);
        inventorySlots.Add(CreateSlot(item1, 1));
        inventorySlots.Add(CreateSlot(item2, 1));
        Initialized = true;
        Debug.Log("Shop Inventory Initialized");
    }

    public void Init()
    {
        inventorySlots.Clear();
    }

    private ItemSlot CreateSlot(ItemData item, int quantity)
    {
        var slot = new ItemSlot();
        slot.Set(item, quantity);
        return slot;
    }

    public List<ItemSlot> GetInventorySlots() => inventorySlots;

    public bool AddToInventory(ItemData item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.IsEmpty)
            {
                slot.Set(item, 1);
                return true;
            }
        }
        return false;
    }

    public bool RemoveFromInventory(ItemData item)
    {
        foreach (var slot in inventorySlots)
        {
            if (!slot.IsEmpty && slot.Item == item)
            {
                slot.Clear();
                return true;
            }
        }
        return false;
    }
}
