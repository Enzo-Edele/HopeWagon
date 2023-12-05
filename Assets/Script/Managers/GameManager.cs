using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject player;
    public GridBoard gridBoard;
    public Vector2Int size;
    public GameObject[] railPrefabs;

    public GameObject tileCopy;

    public SaveLoadMenu saveLoadMenu;

    public InGameUI gameUI;
    public GameTile selectedTile;
    public GameTile selectedTileBIS; //to remove

    public string[] stationNameGeneratorPull = {"Montparnasse", "St Jean", "Du Sud", "De l'Ouest",
        "Austerlitz", "Stalingrad", "Lavoisier" };
    [SerializeField]int nameIndex = 0;
    [SerializeField] int nameLooped = 1;

    public Color[] colorArray = { Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.white };
    public int networkNumber; //fonction pour réatribuer les numéros quand liste remove

    public List<IndustryScriptable> industryTypes = new List<IndustryScriptable>(); 
    public List<RessourceScriptable> ressourceSample = new List<RessourceScriptable>();

    //to scrap
    public List<Material> Industrymats = new List<Material>();

    public GameObject factoryPrefab; //to remove
    public GameObject stationPrefab; //to remove 
    public GameObject trainPrefab;
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            saveLoadMenu.gameObject.SetActive(true);
        if (Input.GetKeyDown(KeyCode.C))
            saveLoadMenu.gameObject.SetActive(false);

        if(selectedTile != null)
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
