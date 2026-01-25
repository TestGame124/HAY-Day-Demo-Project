using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelectButton : DragableBase
{
    public ItemDataSO itemData; // Assign your building here

    private PlacementManager manager;

    void Start()
    {
        manager =  FindFirstObjectByType<PlacementManager>();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (manager != null) manager.StartDraggingFromUI(itemData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (manager != null) manager.EndDraggingFromUI();
    }

}
