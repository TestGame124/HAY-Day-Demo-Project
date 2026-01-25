using System.Collections;
using UnityEngine;

public class CropBehaviour : MonoBehaviour, ITapeable
{

    [SerializeField] bool seedNotNeeded;
    [Space]
    public FoodItemData cropData;
    public CropState CropState = CropState.Empty;

    [Space]
    public float growthRate = 1.0f;
    public float maxTimeToMature = 10.0f;

    public GameObject[] cropModels;


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

    public bool Plant(FoodItemData foodItem)
    {
        if(growthCoroutine != null)
            StopCoroutine(growthCoroutine);

        if (CropState != CropState.Empty)
            return false;

        cropData = foodItem;
        growthCoroutine = StartCoroutine(Grow());
        return true;
    }

    private IEnumerator Grow()
    {
        float elapsedTime = 0f;
        int growthSage = 1;

        CropState = CropState.InProgress;

        while (elapsedTime < maxTimeToMature)
        {
            // Simulate growth over time
            elapsedTime += Time.deltaTime * growthRate;


            float stagesInPercent = maxTimeToMature/cropModels.Length;

            if (elapsedTime >= stagesInPercent * growthSage && growthSage < cropModels.Length)
            {
                for (int i = 0; i < cropModels.Length; i++)
                {
                    cropModels[i].SetActive(i == (growthSage - 1));
                }
                growthSage++;
            }
            yield return null; 
        }

        for (int i = 0; i < cropModels.Length; i++)
        {
            cropModels[i].SetActive(i == (growthSage - 1));
        }

        CropState = CropState.ReadyToHarvest;
        effectOnMature.SetActive(true);
        // Crop has matured
        Debug.Log("Crop has matured!");
    }

    public void OnTap()
    {
        Debug.Log("Crop tapped!");
        if (CropState == CropState.ReadyToHarvest)
        {
            Debug.Log("Crop harvested!");
            // Reset crop
            CropState = CropState.Empty;
            for (int i = 0; i < cropModels.Length; i++)
            {
                cropModels[i].SetActive(false);
            }

            EffectsManager.Instance.SpawnItemPickerEffect(cropData.icon, transform.position + Vector3.up * 0.5f);
            effectOnMature.SetActive(false);
            if (seedNotNeeded)
            {
                Plant(cropData);
            }
        }
    }


}

public enum CropState
{
    Empty,
    InProgress,
    ReadyToHarvest
}
