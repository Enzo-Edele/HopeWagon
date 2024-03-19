using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool Debbug;

    public GridBoard gridBoard;
    public Vector2Int size;
    public GameObject[] railPrefabs;
    public PollutionManager pollutionManager;

    public PlayerData playerData;

    public GameObject tileCopy; //make a struct

    public UIGridEditor gridEditor;
    public CameraController cameraController;
    public UIManagerMainMenu mainMenuUI;
    public UIManagerMenu pauseMenu;
    public UIManagerInGame gameUI;
    public GameTile selectedTile;

    public string[] stationNameGeneratorPull = {"Montparnasse", "St Jean", "Du Sud", "De l'Ouest",
        "Austerlitz", "Lavoisier" };
    [SerializeField]int nameIndex = 0;
    [SerializeField] int nameLooped = 1;

    public Color[] colorArrayTile = { new Color(0, 0, 0) };
    public Color[] colorArrayNetwork = { Color.red };

    public List<IndustryScriptable> industryTypes = new List<IndustryScriptable>();
    public List<PollutedScriptable> industryPollutedTypes = new List<PollutedScriptable>();
    public List<RessourceScriptable> ressourceTypes = new List<RessourceScriptable>();
    public List<ContractScriptable> contratTypes = new List<ContractScriptable>();

    public GameObject factoryPrefab;
    public GameObject pollutedFactoryPrefab;
    public GameObject cleanerPrefab;
    public GameObject stationPrefab;
    public GameObject trainPrefab;
    public GameObject routePrefab;
    public List<GameObject> wagonTemplate = new List<GameObject>(); //to remove 
    public GameObject prefabPollutionParticle;

    public Color polluted, depolluted;

    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        mainMenu,
        pause,
        inGame,
        tuto
    }
    public static GameState gameState { get; private set; }

    void Awake()
    {
        Instance = this;
        gridBoard.Initialize(size);
    }
    private void Start()
    {
        mainMenuUI.Load("SaveStart");
        ChangeGameState(GameState.mainMenu);
        //pollutionManager.Init();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            InterractSaveLoadMenu();
        //if (Input.GetKeyDown(KeyCode.E)) //to disable for demo build
            //OpenCloseEditMode();

        //if (Input.GetKeyDown(KeyCode.P))
            //gridBoard.PaintAllTile(colorArrayTile[3]);
        //if (Input.GetKeyDown(KeyCode.M))
            //gridBoard.PaintAllTile(colorArrayTile[0]);

        if (selectedTile != null)
            selectedTile.UpdateUI(gameUI);
    }

    public void ChangeGameState(GameState state)
    {
        gameState = state;
        switch (gameState)
        {
            case GameState.inGame:
                Time.timeScale = 1.0f;
                pauseMenu.ActivateHelpButton(true);
                break;
            case GameState.tuto:
                break;
            case GameState.mainMenu:
                Time.timeScale = 0.0f;
                break;
            case GameState.pause:
                Time.timeScale = 0.0f;
                pauseMenu.ActivateHelpButton(false);
                TutoManager.Instance.CloseTuto();
                break;
        }
    }

    public void InterractSaveLoadMenu() {
        pauseMenu.ActivateMenu();
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
