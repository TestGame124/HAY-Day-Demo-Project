using UnityEngine;

[CreateAssetMenu(fileName = "FoodItemDatabase", menuName = "Prototype/Food Item Database", order = 1)]
public class ItemDatabase : ScriptableObject
{
    public GrowthableItemsData[] CropsProducts;
    public RecipeItemData[] Recipies;
}
