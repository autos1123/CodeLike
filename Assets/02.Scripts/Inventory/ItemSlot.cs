using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemSlot
{
    public ItemData Item { get; private set; }
    public int Quantity { get; private set; }

    public bool IsEmpty => Item == null;

    public void Set(ItemData item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public void Clear()
    {
        Item = null;
        Quantity = 0;
    }
}
