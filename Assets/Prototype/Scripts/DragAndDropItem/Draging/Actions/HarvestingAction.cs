using UnityEngine;
[CreateAssetMenu(menuName = "Interactions/Harvesting")]
public class HarvestingAction : DragInteractionStrategy
{
    public override void Interact(GameObject target, ItemData itemData)
    {
        var soil = target.GetComponent<CropBehaviour>();
        
        if (soil != null && soil.ProducerState == ProducerState.ReadyToHarvest)
        {
            soil.Gather();
        }
    }
}
