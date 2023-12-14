using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridBoard gridBoard;
    public Vector2Int size;
    public GameObject[] railPrefabs;

    public PlayerData playerData;

    public GameObject tileCopy;

    public UIGridEditor gridEditor;
    public SaveLoadMenu saveLoadMenu;
    public InGameUI gameUI;
    public GameTile selectedTile;
    public GameTile selectedTileBIS; //to remove

    public string[] stationNameGeneratorPull = {"Montparnasse", "St Jean", "Du Sud", "De l'Ouest",
        "Austerlitz", "Stalingrad", "Lavoisier" };
    [SerializeField]int nameIndex = 0;
    [SerializeField] int nameLooped = 1;

    public Color[] colorArrayTile = { new Color(0, 0, 0) };
    public Color[] colorArrayNetwork = { Color.red };

    public List<IndustryScriptable> industryTypes = new List<IndustryScriptable>(); 
    public List<RessourceScriptable> ressourceTypes = new List<RessourceScriptable>();
    public List<ContractScriptable> contratTypes = new List<ContractScriptable>();

    //to scrap
    public List<Material> Industrymats = new List<Material>(); //move to scriptable

    public GameObject factoryPrefab; //to remove ???
    public GameObject stationPrefab; //to remove ???
    public GameObject trainPrefab;
    public GameObject routePrefab;
    public List<GameObject> wagonTemplate = new List<GameObject>(); //to remove 

    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        menu,
        pause,
        inGame
    }
    public static GameState gameState { get; private set; }

    void Awake()
    {
        Instance = this;
        gridBoard.Initialize(size);
    }
    private void Start()
    {
        saveLoadMenu.Load("Save1"); //use for demo build
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            InterractSaveLoadMenu();
        if (Input.GetKeyDown(KeyCode.E)) //to disable for demo build
            OpenCloseEditMode();

        if (Input.GetKeyDown(KeyCode.P))
            gridBoard.PaintAllTile(colorArrayTile[3]);
        if (Input.GetKeyDown(KeyCode.M))
            gridBoard.PaintAllTile(colorArrayTile[0]);

        if (selectedTile != null)
            selectedTile.UpdateUI(gameUI);
        if(selectedTileBIS != null)
            selectedTileBIS.UpdateUIBIS(gameUI);
    }

    public void ChangeGameState(GameState state)
    {
        gameState = state;
        switch (gameState)
        {
            case GameState.inGame:
                break;
            case GameState.menu:
                break;
            case GameState.pause:
                break;
        }
    }

    public void InterractSaveLoadMenu() {
        saveLoadMenu.ActivateMenu();
    }

    public void OpenCloseEditMode()
    {
        gridEditor.ActivateEditMode();
    }

    public string GiveStationName()
    {
        string stationName = stationNameGeneratorPull[nameIndex] + " " + nameLooped ;
        nameIndex += 1;
        if (nameIndex > stationNameGeneratorPull.Length - 1)
        {
            nameIndex = 0;
            nameLooped++;
        }
        return stationName;
    }
}
