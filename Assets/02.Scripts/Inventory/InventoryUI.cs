using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : UIBase
{
    public override string UIName => "InventoryUI"; 
    
    public Inventory inventory;
    
    public SlotUI[] inventorySlotUIs; // 중간 16칸
    
    public override void Open()
    {
        base.Open();
        RefreshUI();
    }

    public void RefreshUI()
    {
        for(int i = 0; i < inventorySlotUIs.Length; i++)
            inventorySlotUIs[i].Set(inventory.inventorySlots[i]);
    }

}
