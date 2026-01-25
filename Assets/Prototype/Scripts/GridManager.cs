using UnityEngine;

public class GridManager : MonoBehaviour
{

    public int alreadyPurchasedLand = 0;
    [Space]
    public int width = 10;
    public int height = 10;
    public float tileSize = 10f;
    public GameObject landPrefab;

    private LandTile[,] grid;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new LandTile[width, height];
        int tileCountX = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0.1f, y * tileSize);
                GameObject obj = Instantiate(landPrefab, transform.position + pos, Quaternion.identity);

                LandTile tile = obj.GetComponent<LandTile>();
                tile.Setup(x, y, this);
                grid[x, y] = tile;
                
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
               
                LandTile tile = GetTile(x,y);
                if(tileCountX < alreadyPurchasedLand)
                    tile.PurchaseLand();
            }
            tileCountX++;
        }

    }

    // Helper to get a tile safely without crashing if out of bounds
    public LandTile GetTile(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return grid[x, y];
        }
        return null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                LandTile tile = hit.collider.GetComponent<LandTile>();
                if (tile != null)
                {
                    tile.PurchaseLand();
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                LandTile tile = hit.collider.GetComponent<LandTile>();
                if (tile != null)
                {
                    tile.UnPurchaseLand();
                }
            }
        }
        
        
    }
}
