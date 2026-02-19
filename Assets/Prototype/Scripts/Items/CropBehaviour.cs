using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;

public class CropBehaviour : MonoBehaviour, ITapeable
{
    public CropState CropState = CropState.Empty;

    [SerializeField] ItemDatabase itemDatabase;
    [SerializeField] bool seedNotNeeded;
    [Space]

    public GrowthableItemsData cropData;
    [Space]
    public SpriteRenderer mainPlant;
    [Space]
    public float growthRate = 1.0f;
    public float maxTimeToMature = 10.0f;

    Coroutine growthCoroutine;

    [Header("VFX")]
    [SerializeField] GameObject effectOnMature;


    private void OnEnable()
    {
        if (seedNotNeeded)
        {
            Plant(cropData);
        }
    }

    public bool Plant(GrowthableItemsData foodItem)
    {
        if(growthCoroutine != null)
            StopCoroutine(growthCoroutine);

        if (CropState != CropState.Empty)
            return false;

        cropData = foodItem;

        mainPlant.gameObject.SetActive(true);

        growthCoroutine = StartCoroutine(Grow());
        return true;
    }

    private IEnumerator Grow()
    {
        float elapsedTime = 0f;
        int growthSage = 1;
        mainPlant.sprite = cropData.itemStagesSprites[growthSage - 1];

        CropState = CropState.InProgress;

        int totalStages = cropData.itemStagesSprites.Length;

        while (elapsedTime < maxTimeToMature)
        {
            // Simulate growth over time
            elapsedTime += Time.deltaTime * growthRate;

            float stagesInPercent = maxTimeToMature/ totalStages;

            if (elapsedTime >= stagesInPercent * growthSage && growthSage < totalStages)
            {
                ChangePlantState(growthSage);

                growthSage++;
            }
            yield return null; 
        }

        ChangePlantState(growthSage);

        CropState = CropState.ReadyToHarvest;
        effectOnMature.SetActive(true);
        // Crop has matured
        Debug.Log("Crop has matured!");
    }

    private void ChangePlantState(int stage)
    {
        for (int i = 0; i < stage; i++)
        {
            mainPlant.sprite = cropData.itemStagesSprites[stage - 1];
        }
    }
    public void OnTap()
    {

        switch(CropState)
        {
            case CropState.Empty:
                Seed();
                break;
            case CropState.ReadyToHarvest:
                //Harvest();
                HarvestUI();
                break;
        }
       
    }

    private void Seed()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        UIController.instance.OpenCropsInfoButtons(screenPos, 
            itemDatabase);
    }

    private void HarvestUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        UIController.instance.OpenHarvestingUI(screenPos);
    }

    public void Harvest()
    {
        if(CropState != CropState.ReadyToHarvest)
            return;
        mainPlant.gameObject.SetActive(false);
        // Reset crop
        CropState = CropState.Empty;

        EffectsManager.Instance.SpawnItemPickerEffect(cropData.icon, transform.position + Vector3.up * 0.5f);
        effectOnMature.SetActive(false);

        InventorySystem barn = InventorySystem.Get(InventoryType.Crops_Inventory);

        barn?.AddItem(new Item(cropData, 1));


        if (seedNotNeeded)
        {
            Plant(cropData);
        }
    }
    

    
    public bool IsEmpty() => CropState == CropState.Empty;

}

public enum CropState
{
    Empty,
    InProgress,
    ReadyToHarvest
}
