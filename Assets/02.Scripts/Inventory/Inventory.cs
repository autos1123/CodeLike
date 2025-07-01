using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private ItemDataTable itemDataTable;
    
    public List<ItemSlot> equippedSlots = new List<ItemSlot>(); // 장착 슬롯 (4칸)
    public List<ItemSlot> inventorySlots = new List<ItemSlot>(16);
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => TableManager.Instance.loadComplete);
        itemDataTable = TableManager.Instance.GetTable<ItemDataTable>();
        Init();

        // 테스트 아이템 추가
        var item = itemDataTable.GetDataByID(6000);
        AddToInventory(item);
    }
    public void Init()
    {
        inventorySlots.Clear();
        for (int i = 0; i < 16; i++)
            inventorySlots.Add(new ItemSlot());
    }
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
}
