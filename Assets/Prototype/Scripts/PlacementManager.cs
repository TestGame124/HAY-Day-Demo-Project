using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class PlacementManager : MonoBehaviour
{
    [Header("UI")]
    private ConfrimationWorldUIBehaviour currentButtonsUI;
    [Space]
    [SerializeField] ConfrimationWorldUIBehaviour SelectedItemUI;
    [SerializeField] ConfrimationWorldUIBehaviour confirmationItemUI;

    //[SerializeField] ConfrimationWorldUIBehaviour confirmationUI;
    [Header("Settings")]
    public ItemDataSO ItemData;
    public GameObject highlightPrefab;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public float cellSize = 2f;

    private BuildingInfo currentGhost;
    private BuildingInfo currentBuildingInfo;
    private float currentRotation = 0f; // Track rotation

    private List<GameObject> highlightList = new List<GameObject>();
    private float yAxisOffset = 7;

    bool isPlacing = false;
    bool isEditing;
    void Update()
    {

        if(GameManager.Instance.CurrentGameState != GameState.EditMode)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.B)) StartPlacing(ItemData);

        if(Input.GetMouseButtonDown(0))
        {
            SelectPlacedItem();
        }

        if (currentGhost != null)
        {
            // Rotate with 'R'
            if (Input.GetKeyDown(KeyCode.R)) RotateGhost();

            MoveGhost();

        }
    }

    public void StartPlacing(ItemDataSO prefab)
    {
        CancelPlacement();

        ItemData = prefab;
        currentGhost = Instantiate(ItemData.ItemPrefab);
        currentBuildingInfo = currentGhost;

        // Disable colliders on ghost
        foreach (var c in currentGhost.GetComponentsInChildren<Collider>()) c.enabled = false;

        foreach (Vector2Int pos in currentBuildingInfo.footprint)
        {
            GameObject hl = Instantiate(highlightPrefab);

            hl.transform.localScale = new Vector3(cellSize * 0.9f, cellSize * 0.9f, 1);
            highlightList.Add(hl);
        }

        currentGhost.transform.position = new Vector3(0, -1000f, 0);
    }

    void RotateGhost()
    {
        Debug.Log("Rotate Ghost Called");
        if (currentBuildingInfo.isSprite)
        {

            currentBuildingInfo.FlipObject();
            UpdateGhostRotation();

            return;
        }
        currentRotation += 90f;
        currentGhost.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        UpdateGhostRotation();
    }

    void MoveGhost()
    {

        if(EventSystem.current.IsPointerOverGameObject())
            return;


        if (Input.GetMouseButton(0))
        {
            currentButtonsUI.gameObject.SetActive(false);
            isPlacing = false;
        }else if(Input.GetMouseButtonUp(0))
        {
            EndDraggingFromUI();
            return;
        }

        if (isPlacing) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 snapPos = GetGridPosition(hit.point);
            currentGhost.transform.position = new Vector3(snapPos.x, yAxisOffset, snapPos.z);

            currentGhost.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            List<Vector2Int> shape = currentBuildingInfo.GetRotatedFootprint(currentRotation);

            if (currentGhost.isSprite)
            {
                shape = currentBuildingInfo.GetFootprint(currentGhost.spriteRenderer.flipX);
            }

            for (int i = 0; i < highlightList.Count; i++)
            {
                Vector2Int offset = shape[i];

                Vector3 hlPos = snapPos + new Vector3(offset.x * cellSize, 0.05f, offset.y * cellSize);

                highlightList[i].transform.position = hlPos;
                highlightList[i].transform.rotation = Quaternion.Euler(90, 0, 0); // Flat on ground
            }

            
                
            bool isValid = IsFootprintValid(snapPos, currentBuildingInfo, currentRotation);

            Color color = isValid ? Color.green : Color.red;

            if (!isEditing)
            {
                SetGhostColor(color);
            }

            foreach (var hl in highlightList)
            {
                hl.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.5f);
            }
        }
    }
   

    private void UpdateGhostRotation()
    {

        Vector3 snapPos = currentGhost.transform.position - (Vector3.up * yAxisOffset);
        List<Vector2Int> shape = currentBuildingInfo.GetRotatedFootprint(currentRotation);
        if (currentGhost.isSprite)
        {
            shape = currentBuildingInfo.GetFootprint(currentGhost.spriteRenderer.flipX);
        }
        // 2. Move each highlight square to the correct offset
        for (int i = 0; i < highlightList.Count; i++)
        {
            Vector2Int offset = shape[i];

            // Calculate world position for this specific square
            Vector3 hlPos = snapPos + new Vector3(offset.x * cellSize, 0.05f, offset.y * cellSize);

            highlightList[i].transform.position = hlPos;
            highlightList[i].transform.rotation = Quaternion.Euler(90, 0, 0); // Flat on ground
        }

        bool isValid = IsFootprintValid(snapPos, currentBuildingInfo, currentRotation);
        Color color = isValid ? Color.green : Color.red;

        if (!isEditing)
        {
            SetGhostColor(color);
        }
        foreach (var hl in highlightList)
        {
            hl.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }


    void PlaceItem()
    {
        if (currentGhost == null) return;

        if (IsFootprintValid(currentGhost.transform.position, currentBuildingInfo, currentRotation))
        {
        isPlacing = false;
            currentButtonsUI.gameObject.SetActive(false);
            
            BuildingInfo item = Instantiate(ItemData.ItemPrefab,new Vector3(currentGhost.transform.position.x, 0, currentGhost.transform.position.z), Quaternion.Euler(0, currentRotation, 0));
            
            if(item.isSprite)
                item.spriteRenderer.flipX = currentBuildingInfo.spriteRenderer.flipX;
            item.footprint = currentBuildingInfo.footprint;
            item.itemData = ItemData;
            item.isPlaced = true;
            item.UpdatePosition();

            if(isEditing)
            {
                isEditing = false;
                foreach (var c in item.GetComponentsInChildren<Collider>())
                {
                    c.enabled = true;

                }
            }
            CancelPlacement();
        }
    }


    


    void SelectPlacedItem()
    {
        if(currentGhost != null)
            return;
        Debug.Log("Select Placed Item");

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, obstacleLayer))
        {
            Debug.Log(hit.collider.name);
            BuildingInfo item = hit.collider.GetComponentInParent<BuildingInfo>();
            if(item != null && item.isPlaced)
            {
                isEditing = true;

                UpdateButtonUI();

                ItemData = item.itemData;
                currentGhost = item;
                currentBuildingInfo = currentGhost;

                // Disable colliders on ghost
                foreach (var c in currentGhost.GetComponentsInChildren<Collider>())
                {
                    c.enabled = false;
                    Debug.Log("Collider Close");
                }
                foreach (Vector2Int pos in currentBuildingInfo.footprint)
                {
                    GameObject hl = Instantiate(highlightPrefab);

                    hl.transform.localScale = new Vector3(cellSize * 0.9f, cellSize * 0.9f, 1);
                    highlightList.Add(hl);
                }
            }
        }
    }

    bool IsFootprintValid(Vector3 centerPos, BuildingInfo info, float rotation)
    {

        List<Vector2Int> shape = info.GetRotatedFootprint(rotation);

        if(info.isSprite)
        {
            shape = info.GetFootprint(info.spriteRenderer.flipX);
        }

        foreach (Vector2Int offset in shape)
        {
            // Calculate world position of this specific tile
            Vector3 cellPos = centerPos + new Vector3(offset.x * cellSize, 0, offset.y * cellSize);

            // A. Check Ownership (Raycast down)
            Debug.Log("BEFORE Land Owned at: " + cellPos);

            if (!CheckLandOwnership(cellPos)) return false;
            Debug.Log("Land Owned at: " + cellPos);
            // B. Check Obstacles (Sphere check)
            if (Physics.CheckSphere(cellPos, cellSize * 0.4f, obstacleLayer)) return false;
        }

        return true;
    }

    public Vector3 GetGridPosition(Vector3 hitPosition)
    {
        float x = Mathf.Round(hitPosition.x / cellSize) * cellSize;
        float z = Mathf.Round(hitPosition.z / cellSize) * cellSize;
        return new Vector3(x, hitPosition.y, z);
    }

    bool CheckLandOwnership(Vector3 pos)
    {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 10, Vector3.down, out hit, 20f, groundLayer))
        {
            LandTile tile = hit.collider.GetComponentInParent<LandTile>();
            return (tile != null && tile.isOwned);
        }
        return false;
    }

    public void CancelPlacement()
    {
        if (isEditing && currentBuildingInfo.isPlaced)
            currentBuildingInfo.ResetState();
        else
            if (currentGhost != null) Destroy(currentGhost.gameObject);

        if(currentButtonsUI != null) 
            currentButtonsUI.gameObject.SetActive(false);
        isPlacing = false;

        foreach (var hl in highlightList)
        {
            if (hl != null) Destroy(hl);
        }
        highlightList.Clear();

        ItemData = null;
        currentGhost = null;
        currentRotation = 0f;

        isEditing = false;
    }

    private void SellItem()
    {
        if (currentBuildingInfo != null && isEditing)
        {
            Destroy(currentBuildingInfo.gameObject);
            CancelPlacement();
        }
    }
    void SetGhostColor(Color color)
    {
        Renderer[] renderers = currentGhost.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) r.material.color = new Color(color.r, color.g, color.b, 0.5f);
    }

    public void StartDraggingFromUI(ItemDataSO itemPrefab)
    {
        CancelPlacement();
        UpdateButtonUI();
        StartPlacing(itemPrefab);
    }

    public void EndDraggingFromUI()
    {
        if (currentGhost != null)
        {
            isPlacing = true;
            currentButtonsUI.gameObject.SetActive(true);
            currentButtonsUI.Initialize(currentGhost.transform
                , PlaceItem
                , CancelPlacement
                , RotateGhost
                , isEditing ? SellItem : null);


            //PlaceItem();
            //CancelPlacement();
        }
    }


    private void UpdateButtonUI()
    {
        if (isEditing)
        {
            currentButtonsUI = SelectedItemUI;
        }
        else
            currentButtonsUI = confirmationItemUI;
    }
}