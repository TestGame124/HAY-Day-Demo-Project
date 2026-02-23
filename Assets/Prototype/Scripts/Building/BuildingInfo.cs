using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Analytics;
using System.Collections;
using DG.Tweening;

public class BuildingInfo : MonoBehaviour
{
    public PlaceableItemsDataSO itemData;
    public SpriteRenderer spriteRenderer;

    public bool isPlaced;
    [HideInInspector] public bool isSprite;
    [Space]
    public Transform gridObjects;
    public Transform buttonsPoint;
    [Space]
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] GameObject underConstruction;
    [SerializeField] GameObject readyBuilding;
    [Header("UI")]
    [SerializeField]private TMPro.TextMeshPro timerText;






    private Vector3 currentPosition;
    private Vector3 currentRotation;
    private bool currentFlippedState;
    [HideInInspector]public List<Vector2Int> footprint = new List<Vector2Int>()
    {
        new Vector2Int(0,0)
    };                 
    private void Awake()
    {

        RegisterDependencies();
    }

    private void RegisterDependencies()
    {
        boxCollider = GetComponentInChildren<BoxCollider>(true);
        PlacementManager placementManager = FindFirstObjectByType<PlacementManager>();

        Transform[] gridPositions = gridObjects.GetComponentsInChildren<Transform>(true);

        foreach (Transform go in gridPositions)
        {
            Vector3 gridPos = placementManager.GetGridPosition(go.position);

            Vector2Int gridIntPos = new Vector2Int(Mathf.RoundToInt(gridPos.x / placementManager.cellSize), Mathf.RoundToInt(gridPos.z / placementManager.cellSize));

            footprint.Add(gridIntPos);
        }
    }

    private void OnEnable()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        isSprite = spriteRenderer != null;
    }

    public List<Vector2Int> GetFootprint(bool isFlipped)
    {
        if (!isFlipped) return footprint; // Return original

        List<Vector2Int> newFootprint = new List<Vector2Int>();

        foreach (Vector2Int pos in footprint)
        {
            newFootprint.Add(new Vector2Int(pos.y, pos.x));
        }
        if(boxCollider != null)
            boxCollider.size = new Vector3(boxCollider.size.z, boxCollider.size.y, boxCollider.size.x);
        return newFootprint;
    }
    public List<Vector2Int> GetRotatedFootprint(float rotationY)
    {
        List<Vector2Int> rotated = new List<Vector2Int>();

            int rotIndex = Mathf.RoundToInt(rotationY / 90f) % 4;
        if (rotIndex < 0) rotIndex += 4;

        foreach (Vector2Int pos in footprint)
        {
            Vector2Int newPos = pos;
            
            for (int i = 0; i < rotIndex; i++)
            {
                newPos = new Vector2Int(newPos.y, -newPos.x);
            }
            rotated.Add(newPos);
        }
        return rotated;
    }

    public void UpdatePosition()
    {
        currentPosition = transform.position;
        currentFlippedState = isSprite ? spriteRenderer.flipX : false;

        currentRotation = transform.rotation.eulerAngles;
    }
    public void ResetState()
    {
        transform.position = currentPosition;
        transform.rotation = Quaternion.Euler(currentRotation);

        if (isSprite)
            spriteRenderer.flipX = currentFlippedState;
        

        if (boxCollider != null)
            boxCollider.enabled = true;

    }

    public void StartBuilding()
    {
        if (itemData.CanPlaceImmedietly())
        {
            BuildingState(true);
            return;
        }

        BuildingState(false);

        StartCoroutine(BuildingInProcess());
    }

    IEnumerator BuildingInProcess()
    {
        int currentTime = itemData.ConstructionTimeInSeconds;
        while (currentTime > 0)
        {

            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            // Use "00" format specifier to ensure two digits with leading zeros
            string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);
            timerText.text = formattedTime;
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        BuildingState(true);
        readyBuilding.transform.DOScale(Vector3.one * 1, 0.3f).From(0.8f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            readyBuilding.transform.localScale = Vector3.one;
        });
    }

    private void BuildingState(bool ready)
    {
        underConstruction.SetActive(!ready);
        readyBuilding.SetActive(ready);
    }

    public Transform GetButtonsPoint()
    {
        return buttonsPoint;
    }

    public void FlipObject()
    {
        if (isSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            return;
        }
    }
}