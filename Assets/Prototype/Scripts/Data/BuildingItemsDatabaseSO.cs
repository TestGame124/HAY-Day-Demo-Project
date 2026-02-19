using UnityEngine;


[CreateAssetMenu(fileName = "ItemsDatabaseSO", menuName = "ScriptableObjects/ItemsDatabaseSO", order = 1)]
public class BuildingItemsDatabaseSO : ScriptableObject
{
    public PlaceableItemsDataSO[] ItemsData;
}
