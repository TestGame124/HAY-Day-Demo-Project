using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public static ItemController instance;

    [SerializeField] public ItemDatabase itemsDatabase;
    private static Dictionary<ItemType, GrowthableItemsData> cropsItemsDict = new();
    private static Dictionary<ItemType,RecipeItemData> recipiesItemsDict = new();


    private static Dictionary<ItemType, ItemData> allItemsDict = new();
    public void Initialize()
    {
        instance = this;
        RegisterItems();

    }
    private void RegisterItems()
    {
        for (int i = 0; i < itemsDatabase.Recipies.Length; i++)
        {
            RecipeItemData tempItem = itemsDatabase.Recipies[i];
            if (!recipiesItemsDict.ContainsKey(tempItem.recipeType))
            {
                recipiesItemsDict.Add(tempItem.recipeType, tempItem);
            }
        }

        for (int i = 0; i < itemsDatabase.CropsProducts.Length; i++)
        {
            GrowthableItemsData tempItem = itemsDatabase.CropsProducts[i];
            if (!cropsItemsDict.ContainsKey(tempItem.cropType))
            {
                cropsItemsDict.Add(tempItem.cropType, tempItem);
            }
        }
    }

    public static RecipeItemData GetRecipe(ItemType type)
    {
        if (!recipiesItemsDict.ContainsKey(type))
        {
            Debug.LogError("No Item Type Of : " + type + " Found!");
            return null;
        }
        return recipiesItemsDict[type];
    }
    public static GrowthableItemsData GetCropItem(ItemType type)
    {
        if (!cropsItemsDict.ContainsKey(type))
        {
            Debug.LogError("No Item Type Of : " + type + " Found!");
            return null;
        }
        return cropsItemsDict[type];
    }

    public static T GetItem<T>(ItemType type) where T : ItemData
    {
        // TryGetValue fetches the item in a single, highly-optimized step
        if (allItemsDict.TryGetValue(type, out ItemData itemData))
        {
            // The 'as' keyword attempts a cast. If the item is the wrong type, it safely returns null.
            T castedItem = itemData as T;
            if (castedItem != null)
            {
                return castedItem;
            }

            Debug.LogError($"Item '{type}' was found, but it is a {itemData.GetType().Name}, not a {typeof(T).Name}!");
            return null;
        }

        Debug.LogError($"No Item Type Of: {type} Found!");
        return null;
    }


    public int GetFoodItemsCount()
    {
        return itemsDatabase.CropsProducts.Length;
    }
}
