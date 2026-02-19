using JetBrains.Annotations;
using Newtonsoft.Json.Bson;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlacementManager placementManager;
    public GridManager gridManager;
    public GameState CurrentGameState { get; private set; } = GameState.PlayMode;
    

    public GameObject gridObject;

    public GameObject itemsUI;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActivatePlayMode();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo,Mathf.Infinity))
            {
                if (hitInfo.collider != null)
                {
                    Debug.Log("Item Found");
                    var tapeable = hitInfo.collider.TryGetComponent(out ITapeable tapItem);
                    if (tapeable)
                    {
                        tapItem?.OnTap();
                    }
                }
            }
        }
    }

    public void ChangeGameState(GameState gameState)
    {
        CurrentGameState = gameState;
    }



    public void ActivateEditMode()
    {
        ChangeGameState(GameState.EditMode);
        gridObject.SetActive(true);
        itemsUI.SetActive(true);


    }

    public void ActivatePlayMode()
    {
        ChangeGameState(GameState.PlayMode);
        gridObject.SetActive(false);
        itemsUI.SetActive(false);

        placementManager.CancelPlacement();

    }
}

public enum GameState
{
    PlayMode,
    EditMode
}
