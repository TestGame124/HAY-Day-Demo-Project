using UnityEngine;

[CreateAssetMenu(menuName = "Interactions/Planting")]
public class PlantingAction : DragInteractionStrategy
{
    public override void Interact(GameObject target, ItemData itemData)
    {
        // 1. Try to find the component we care about
        var soil = target.GetComponent<CropBehaviour>();

        // 2. Validate
        if (soil != null && soil.IsEmpty() && itemData != null)
        {
            // 3. Execute
            GrowthableItemsData growthableItemData = itemData as GrowthableItemsData;
            soil.Plant(growthableItemData);
        }
    }
}