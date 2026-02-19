using UnityEngine;
[CreateAssetMenu(menuName = "Interactions/Harvesting")]
public class HarvestingAction : DragInteractionStrategy
{
    public override void Interact(GameObject target, ItemData itemData)
    {
        var soil = target.GetComponent<CropBehaviour>();
        Debug.Log("Before Harvesting crop...");

        if (soil != null && soil.CropState == CropState.ReadyToHarvest)
        {
            Debug.Log("Harvesting crop...");
            soil.Harvest();
        }
    }
}
