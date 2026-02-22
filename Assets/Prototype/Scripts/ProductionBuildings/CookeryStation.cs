using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookeryStation : MonoBehaviour, ITapeable
{
    [SerializeField] CookStationState cookStationState;
    [Space]
    [SerializeField] SpriteRenderer cookStationImage;
    [Space]
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite busySprite;

    [Space]
    [SerializeField] CookingRecipe[] recipies;
    

    [SerializeField] List<Coroutine> recipiesCoroutines;

    [SerializeField]
    public List<CookRecipeData> recipiesInProgress;


    public static Action OnStartCooking;

    [SerializeField] int MaxCookRecipies;
    public Transform centerPoint;

   
    public void Initialize()
    {
        

    }

    public void OnTap()
    {
        //cookPanel = UIGame.GetUI().cookingUIPanel;

        //cookPanel.gameObject.SetActive(true);
        //cookPanel.Setup(recipies, this);
        //MaxCookRecipies = cookPanel.GetSlotsCount();

        StartCooking(recipies[0],InventorySystem.Get(InventoryType.Barn));

        Initialize();
    }


    public bool StartCooking(CookingRecipe recipe, InventorySystem inventory)
    {
        if (recipiesInProgress.Count >= MaxCookRecipies)
        {
            return false;
        }
        CookRecipeData newRecipe = new CookRecipeData(this, recipe, inventory);


        if (recipiesInProgress.Count == 0)
            newRecipe.active = true;
        else
            newRecipe.active = false;

        recipiesInProgress.Add(newRecipe);
        OnStartCooking?.Invoke();

        StartCoroutine(newRecipe.StartCookingTimer(this));
        return true;

    }


    public void RemoveCookRecipeData(CookRecipeData recipeData)
    {
        recipiesInProgress.Remove(recipeData);
    }

    public void ChangeBuildingVisual(bool Active)
    {
        cookStationImage.sprite = Active ? busySprite : normalSprite;
    }
}
public enum CookStationState
{
    Idle,
    Finished
}
[System.Serializable]
public class CookStationData
{
    public CookRecipeData[] recipiesInProgress;

    public CookStationData(CookRecipeData[] recipiesInProgress)
    {
        this.recipiesInProgress = recipiesInProgress;
    }
}

