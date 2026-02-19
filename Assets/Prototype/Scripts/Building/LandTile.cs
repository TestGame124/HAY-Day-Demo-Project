using UnityEngine;

public class LandTile : MonoBehaviour
{
    public int x, y; // Coordinates in the grid
    public bool isOwned = false;

    [Header("Visual References")]
    public GameObject topBorder;
    public GameObject bottomBorder;
    public GameObject leftBorder;
    public GameObject rightBorder;
    [Space]
    [SerializeField] GameObject purchasableVisual;

    private GridManager gridManager;

    [SerializeField] GameObject[] extraObjects;

    int randIndex;

    bool isPurchasable = false;
    public void Setup(int x, int y, GridManager manager)
    {
        this.x = x;
        this.y = y;
        this.gridManager = manager;
        randIndex = Random.Range(0, extraObjects.Length);
        extraObjects[randIndex].SetActive(true);
        UpdateBorders(); // Initial state

        if(x == 0 && y == 0)
        {
            isPurchasable = true;
        }
            purchasableVisual.SetActive(isPurchasable && !isOwned);
    }

    public void PurchaseLand()
    {
        if (isOwned || !isPurchasable) return;

        isOwned = true;

        UpdateBorders();
        UpdateNeighbors();

        UpdateNeighborsPurchasable();
        purchasableVisual.SetActive(false);
    }

    public void UnPurchaseLand()
    {
        isOwned = false;
        topBorder.SetActive(false);
        bottomBorder.SetActive(false);
        leftBorder.SetActive(false);
        rightBorder.SetActive(false);

        UpdateNeighbors();

        UpdateNeighborsUnPurchasable();
    }

    public void UpdateBorders()
    {
        if (!isOwned)
        {
            return;
        }
        extraObjects[randIndex].SetActive(false);
        // Check Top
        LandTile topTile = gridManager.GetTile(x, y + 1);
        bool mergeTop = (topTile != null && topTile.isOwned);
        topBorder.SetActive(!mergeTop); // Hide border if neighbor is owned

        // Check Bottom
        LandTile bottomTile = gridManager.GetTile(x, y - 1);
        bool mergeBottom = (bottomTile != null && bottomTile.isOwned);
        bottomBorder.SetActive(!mergeBottom);

        // Check Left
        LandTile leftTile = gridManager.GetTile(x - 1, y);
        bool mergeLeft = (leftTile != null && leftTile.isOwned);
        leftBorder.SetActive(!mergeLeft);

        // Check Right
        LandTile rightTile = gridManager.GetTile(x + 1, y);
        bool mergeRight = (rightTile != null && rightTile.isOwned);
        rightBorder.SetActive(!mergeRight);
    }
    
    private void UpdateNeighbors()
    {
        // Tell neighbors to re-check their borders now that I am owned
        gridManager.GetTile(x, y + 1)?.UpdateBorders();
        gridManager.GetTile(x, y - 1)?.UpdateBorders();
        gridManager.GetTile(x - 1, y)?.UpdateBorders();
        gridManager.GetTile(x + 1, y)?.UpdateBorders();
    }

    private void UpdateNeighborsPurchasable()
    {    
        gridManager.GetTile(x, y + 1)?.UpdatePurchasable();
        gridManager.GetTile(x, y - 1)?.UpdatePurchasable();
        gridManager.GetTile(x - 1, y)?.UpdatePurchasable();
        gridManager.GetTile(x + 1, y)?.UpdatePurchasable();
    }
    private void UpdateNeighborsUnPurchasable()
    {    
        bool isAnyNeighborOwned = false;

        bool tileOneOwned = gridManager.GetTile(x, y + 1) && gridManager.GetTile(x, y + 1).isOwned;
        bool tileTwoOwned = gridManager.GetTile(x, y - 1) && gridManager.GetTile(x, y - 1).isOwned;
        bool tileThreeOwned = gridManager.GetTile(x - 1, y) && gridManager.GetTile(x - 1, y).isOwned;
        bool tileFourOwned = gridManager.GetTile(x + 1, y) && gridManager.GetTile(x + 1, y).isOwned;
        
        isAnyNeighborOwned = tileOneOwned || tileTwoOwned || tileThreeOwned || tileFourOwned;
        
        isPurchasable = isAnyNeighborOwned;
        purchasableVisual.SetActive(isPurchasable && !isOwned);
        gridManager.GetTile(x, y + 1)?.UpdatePurchasable();
        gridManager.GetTile(x, y - 1)?.UpdatePurchasable();
        gridManager.GetTile(x - 1, y)?.UpdatePurchasable();
        gridManager.GetTile(x + 1, y)?.UpdatePurchasable();
    }

    

    public void UpdatePurchasable()
    {
        if(isPurchasable && isOwned)
        {
            purchasableVisual.SetActive(false);
            return;
        }
        isPurchasable = true;
        purchasableVisual.SetActive(true);

    }


    public GameObject occupiedObject = null; // Stores the building placed here

    public bool CanPlaceObject()
    {
        // We can only place if:
        // 1. We own the land
        // 2. There is nothing already here
        return isOwned && occupiedObject == null;
    }
}
