using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResourcesNodeUIBehaviour : MonoBehaviour
{
    [SerializeField]private CanvasGroup canvasGroup;
    [SerializeField]private ResourceDragableUI resourceDragablePrefab;

    [Space]
    [SerializeField] Transform itemParent;
    internal void Initialize(ItemDatabase itemsDatabase,Action onBeginDrag, Action<GrowthableItemsData> onDrag, Action onEndDrag)
    {
        SpawnItems(itemsDatabase, onBeginDrag, onDrag, onEndDrag);
    }

    public virtual void Initialize()
    {

    }
    

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!IsPointerOverUI())
            {
                gameObject.SetActive(false);
            }
        }        
    }

    private void SpawnItems(ItemDatabase itemsDatabase, Action onBeginDrag, Action<GrowthableItemsData> onDrag, Action onEndDrag)
    {
        Debug.Log("Spawning Items in Resource Node UI" + itemsDatabase.CropsProducts.Length);

        for (int i = itemParent.childCount - 1; i >= 0; i--)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }

        foreach (GrowthableItemsData itemData in itemsDatabase.CropsProducts)
        {
            var resourceUI = Instantiate(resourceDragablePrefab, itemParent);
            resourceUI.Initialize(this, itemData, onBeginDrag, onDrag, onEndDrag);
        }
    }

    public void ToggleNode(bool state)
    {
        canvasGroup.alpha = state ? 1 : 0; // Show or hide the UI
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    
}
