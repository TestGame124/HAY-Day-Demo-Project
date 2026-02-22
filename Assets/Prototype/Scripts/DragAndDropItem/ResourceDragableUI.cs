using Coffee.UIEffects;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceDragableUI : DragableBase
{

    public LayerMask interactableLayers; 
    [Header("Configuration")]
    public DragInteractionStrategy interactionStrategy; // Drag your Logic SO here
    [Space]
    public ResourcesNodeUIBehaviour resourcesParent; // Reference to the UI behaviour
    [Header("Data")]
    [HideInInspector]public GrowthableItemsData itemData;

    [Header("Visuals")]
    public UnityEngine.UI.Image iconImage;
    private GameObject dragIcon; 
    private Canvas canvas;



    private Action onBeginDrag;
    private Action<GrowthableItemsData> onDrag;
    private Action onEndDrag;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();


    }
    public void Initialize(ResourcesNodeUIBehaviour resourceParent,  GrowthableItemsData itemData,Action onBeginDrag = null , Action<GrowthableItemsData> OnDrag = null, Action OnEndDrag=null)
    {
        if (itemData != null)
        {
            this.itemData = itemData;
            iconImage.sprite = itemData.icon;
        }

        this.resourcesParent = resourceParent;

        this.onBeginDrag = onBeginDrag;
        this.onDrag = OnDrag;
        this.onEndDrag = OnEndDrag;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        CreatePreviewIcon();

        onBeginDrag?.Invoke();

    }

    private void CreatePreviewIcon()
    {
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        dragIcon.transform.SetAsLastSibling(); // Draw on top

        Image image = dragIcon.AddComponent<Image>();
        image.sprite = iconImage.sprite;
        image.raycastTarget = false;

        UIEffect addEffect = dragIcon.AddComponent<UIEffect>();
        addEffect.shadowMode = ShadowMode.Outline;


        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0); // Set alpha to 0 immediately

    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = Input.mousePosition;
            this.resourcesParent.ToggleNode(false);
            PerformRaycast();
            //onDrag?.Invoke(itemData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {

        // Destroy the visual icon first
        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 1);
        resourcesParent.gameObject.SetActive(false);
        this.resourcesParent.ToggleNode(true);

        onEndDrag?.Invoke();
        if (dragIcon != null) Destroy(dragIcon);
    }

    private void PerformRaycast()
    {
        if (interactionStrategy == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // We hit SOMETHING on the interactable layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayers))
        {

            interactionStrategy.Interact(hit.collider.gameObject, itemData);
        }
    }
}
