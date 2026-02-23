using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;

public class CropBehaviour : ProducerBase
{

    [SerializeField] bool seedNotNeeded;
    [Space]
    public SpriteRenderer mainPlant;
    [Space]
    public GrowthableItemsData cropData;
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

        if (ProducerState != ProducerState.Empty)
            return false;

        cropData = foodItem;

        mainPlant.gameObject.SetActive(true);

        growthCoroutine = StartCoroutine(InProcess());
        return true;
    }

    protected override IEnumerator InProcess()
    {
        float elapsedTime = 0f;
        int growthSage = 1;
        mainPlant.sprite = cropData.itemStagesSprites[growthSage - 1];

        ProducerState = ProducerState.InProgress;

        int totalStages = cropData.itemStagesSprites.Length;

        while (elapsedTime < maxTimeToGetReady)
        {
            // Simulate growth over time
            elapsedTime += Time.deltaTime * growthRate;

            float stagesInPercent = maxTimeToGetReady/ totalStages;

            if (elapsedTime >= stagesInPercent * growthSage && growthSage < totalStages)
            {
                ChangePlantState(growthSage);

                growthSage++;
            }
            yield return null; 
        }

        ChangePlantState(growthSage);

        ProducerState = ProducerState.ReadyToHarvest;

        effectOnMature.SetActive(true);
    }

   
    public override void OnTap()
    {

        switch(ProducerState)
        {
            case ProducerState.Empty:
                Seed();
                break;
            case ProducerState.ReadyToHarvest:
                //Harvest();
                HarvestUI();
                break;
        }
       
    }

    private void Seed()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        UIController.instance.OpenCropsInfoButtons(screenPos, items);
    }

    private void HarvestUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        UIController.instance.OpenHarvestingUI(screenPos);
    }

    public override void Gather()
    {
        if(ProducerState != ProducerState.ReadyToHarvest)
            return;

        ResetState();

        
        EffectsManager.Instance.SpawnItemPickerEffect(cropData.icon, transform.position + Vector3.up * 0.5f);

        InventorySystem barn = InventorySystem.Get(InventoryType.Crops_Inventory);
        barn?.AddItem(new Item(cropData, 1));


        if (seedNotNeeded)
            Plant(cropData);
    }

    private void ResetState()
    {
        mainPlant.gameObject.SetActive(false);
        ProducerState = ProducerState.Empty;
        effectOnMature.SetActive(false);

    }

    private void ChangePlantState(int stage)
    {
        for (int i = 0; i < stage; i++)
        {
            mainPlant.sprite = cropData.itemStagesSprites[stage - 1];
        }
    }
}

