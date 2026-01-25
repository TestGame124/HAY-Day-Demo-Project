using UnityEngine;

[CreateAssetMenu(fileName = "FoodItemDatabase", menuName = "Prototype/Food Item Database", order = 1)]
public class FoodItemDatabase : ScriptableObject
{
    public FoodItemData[] FoodItemDatas;
}
