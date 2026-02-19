using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static Dictionary<InventoryType, InventorySystem> _instances = new();
    public static InventorySystem Get(InventoryType category)
    {
        if (_instances.ContainsKey(category))
            return _instances[category];

        Debug.LogError($"No Inventory found for category: {category}");
        return null;
    }


    public InventoryType InventoryType;


    private List<Item> items = new List<Item>();

    private Dictionary<ItemType, Item> itemDict = new();

    public int inventorySize = 20;

    public delegate void OnInventoryChanged();
    public static event OnInventoryChanged onInventoryChangedCallback;

    private void Awake()
    {
        if(!_instances.ContainsKey(InventoryType))
        {
            _instances.Add(InventoryType,this);
        }
        else
        {
            Debug.LogError("Multiple Inventories Found!! " + InventoryType);
            Destroy(this);
        }
    }
    public void Initialize()
    {

        // initialize If Needed
        ForTesting();

    }

    void ForTesting()
    {
        Item itemTemp = new Item(ItemController.GetItem<GrowthableItemsData>(ItemType.Wheat), 99);
        Item itemTemp2 = new Item(ItemController.GetItem<GrowthableItemsData>(ItemType.Date), 1);
        Item itemTemp3 = new Item(ItemController.GetItem<GrowthableItemsData>(ItemType.Corn), 10);
        AddItem(itemTemp);
        AddItem(itemTemp2);
        AddItem(itemTemp3);

    }
    public bool AddItem(Item item)
    {
        if (IsFull())
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        bool itemAlreadyExist = false;
        if (ContainsItem(item.type))
        {
            Item inventoryItem = GetItem(item.type);
            inventoryItem.itemsAmount += 1;
            itemAlreadyExist = true;

        }


        if (!itemAlreadyExist)
        {
            items.Add(item);
            itemDict.Add(item.type, item);
        }

        onInventoryChangedCallback?.Invoke();
        return true;
    }

    public bool RemoveItem(Item item)
    {
        if (itemDict.ContainsKey(item.type))
        {

            //if(item.itemsAmount> 1) {
            //    item.itemsAmount -= 1;
            //}
            Item inventoryItem = GetItem(item.type);
            if (inventoryItem.itemsAmount > 1)
            {
                inventoryItem.itemsAmount -= 1;
            }
            else
            {

                items.Remove(inventoryItem);
                itemDict.Remove(inventoryItem.type);

            }
            onInventoryChangedCallback?.Invoke();
            return true;
        }
        Debug.Log(item.type + " not found in inventory.");
        return false;
    }

    public int ItemCount(ItemType itemtype)
    {
        if (GetItem(itemtype) != null)
            return GetItem(itemtype).itemsAmount;
        return 0;
    }

    public bool IsFull()
    {
        return items.Count >= inventorySize;
    }

    public Item GetItem(ItemType type)
    {
        if (itemDict.ContainsKey(type))
            return itemDict[type];
        return null;
    }
    public bool ContainsItem(ItemType item)
    {
        return itemDict.ContainsKey(item);
    }

    public List<Item> GetItemList()
    {
        return items;
    }

    #region Saving Loading

    public InventoryData Save()
    {

        return new InventoryData(items.ToArray(), inventorySize);
    }

    public void Load(InventoryData data)
    {
        //if(data == null)
        //{
        //    data = Save(true);
        //}

        for (int i = 0; i < data.items.Length; i++)
        {
            Item item = data.items[i];
            Item newItem = new(ItemController.GetCropItem(item.type), item.itemsAmount);
            AddItem(newItem);
        }

        inventorySize = data.inventorySize;
    }

    #endregion
}


[System.Serializable]
public class InventoryData
{
    public Item[] items;
    public int inventorySize;


    public InventoryData(Item[] items, int inventorySize)
    {
        this.items = items;
        this.inventorySize = inventorySize;
    }

}



