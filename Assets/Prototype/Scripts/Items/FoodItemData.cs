using UnityEngine;

[CreateAssetMenu(fileName = "CropInfo", menuName = "Prototype/FoodItems")]
public class FoodItemData : ScriptableObject
{
    public FoodItemType foodItemType;
    [Space]
    public Sprite icon;


}
