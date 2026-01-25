using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CropsSelectButton : DragableBase
{
    [Header("Data")]
    public FoodItemData cropToPlant;
    public LayerMask groundLayer; 

    [Header("Visuals")]
    public UnityEngine.UI.Image iconImage;
    private GameObject dragIcon; 
    private Canvas canvas; 
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        // Create a temporary object to drag around
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        dragIcon.transform.SetAsLastSibling(); // Draw on top

        // Add an image component and copy our sprite
        Image image = dragIcon.AddComponent<Image>();
        image.sprite = iconImage.sprite;
        image.raycastTarget = false; // Important: Let clicks pass through to the world!

        // Make it slightly transparent
        Color c = image.color;
        c.a = 0.6f;
        image.color = c;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        // Destroy the visual icon first
        if (dragIcon != null) Destroy(dragIcon);

        // RAYCAST to the world to see what we dropped on
        PlantSeedAtMouse();
    }

    private void PlantSeedAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast against the "Ground" layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Did we hit a LandTile?
            CropBehaviour crop = hit.collider.GetComponentInParent<CropBehaviour>();

            if (crop != null)
            {
                // Try to plant!
                bool success = crop.Plant(cropToPlant);

                if (success)
                {
                    // Optional: Decrease seed count in inventory
                    Debug.Log("Seed planted successfully.");
                }
                else
                {
                    Debug.Log("Cannot plant here (Not owned or already occupied).");
                }
            }
        }
    }
}
