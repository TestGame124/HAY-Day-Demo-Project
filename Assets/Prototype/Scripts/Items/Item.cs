using System;
using UnityEngine;

[Serializable]
public class Item
{
    public ItemData itemDataSO;

    public ItemType type => itemDataSO != null ? itemDataSO.itemType : default;

    public int itemsAmount;

    public Item(ItemData itemDataSO, int itemsAmount)
    {
        this.itemDataSO = itemDataSO;
        this.itemsAmount = itemsAmount;
    }
}