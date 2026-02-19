using UnityEngine;

public abstract class DragInteractionStrategy : ScriptableObject 
{
    // We pass the GameObject we hit, and the Data we are dragging (if any)
    public abstract void Interact(GameObject target, ItemData itemData);
}