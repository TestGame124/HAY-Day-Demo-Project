using UnityEngine;


[CreateAssetMenu(fileName = "ItemsDatabaseSO", menuName = "ScriptableObjects/ItemsDatabaseSO", order = 1)]
public class ItemsDatabaseSO : ScriptableObject
{
    public ItemDataSO[] ItemsData;
}
