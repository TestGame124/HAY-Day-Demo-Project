using UnityEngine;

public class HarvestingUIBehaviour : ResourcesNodeUIBehaviour
{
    [SerializeField] private ResourceDragableUI resourceUI;

    public override void Initialize()
    {
        resourceUI.Initialize(this, null);
    }
}
