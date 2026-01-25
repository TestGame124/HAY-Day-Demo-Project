using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "ScriptableObjects/ItemDataSO", order = 1)]
public class ItemDataSO : ScriptableObject
{
    public BuildingInfo ItemPrefab;
    public Sprite Icon;

    [Space]
    public int LevelRequirement;
    public int MaxAllowed;
    [Space]
    public int Price;
    public int PointsReward;
}
