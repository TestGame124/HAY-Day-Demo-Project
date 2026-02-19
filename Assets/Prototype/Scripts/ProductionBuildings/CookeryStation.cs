using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookeryStation : MonoBehaviour, ITapeable
{

    [SerializeField] CookingRecipe[] recipies;
    

    [SerializeField] List<Coroutine> recipiesCoroutines;

    [SerializeField]
    public List<CookRecipeData> recipiesInProgress;


    public static Action OnStartCooking;

    [SerializeField] int MaxCookRecipies;
    public Transform centerPoint;

    private void Start()
    {
        //foreach (CookingRecipe recipe in recipies)
        //{
        //    OrderSystem.AddAvailableItems(recipe.Results.item);
        //}

    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        //foreach (CookingRecipe recipe in recipies)
        //{
        //    OrderSystem.RemoveAvailableItem(recipe.Results.item);
        //}


    }

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

