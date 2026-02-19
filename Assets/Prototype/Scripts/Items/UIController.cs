using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public Canvas mainCanvas;
    [Space]
    public HarvestingUIBehaviour harvestingUI;
    public ResourcesNodeUIBehaviour cropSelectionUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void OpenCropsInfoButtons(Vector3 position, ItemDatabase itemDatabase, Action onBeginDrag = null, Action<GrowthableItemsData> onDrag = null, Action onEndDrag = null)
    {
        cropSelectionUI.gameObject.SetActive(true);

        float yOffset = 150;

        cropSelectionUI.transform.position = position + (Vector3.up * yOffset);

        cropSelectionUI.Initialize(itemDatabase, onBeginDrag, onDrag, onEndDrag);
    }

    public void OpenHarvestingUI(Vector3 position)
    {
        harvestingUI.gameObject.SetActive(true);
        float yOffset = 150;

        harvestingUI.transform.position = position + (Vector3.up * yOffset);
        harvestingUI.Initialize();
    }

    public void CloseHarvestingUI()
    {
        harvestingUI.gameObject.SetActive(false);
    }
    public void CloseCropsInfoButtons()
    {
        cropSelectionUI.gameObject.SetActive(false);
    }
}
