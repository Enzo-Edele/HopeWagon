using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject player;
    public GridBoard gridBoard;
    Vector2Int size = new Vector2Int(10, 10);
    public GameObject[] railPrefabs;
    public GameObject factoryPrefab; //to remove
    public GameObject stationPrefab; //to remove merge avec l'ancien object station
    public GameObject trainPrefab;

    public InGameUI gameUI;
    public GameTile selectedTile;

    public string[] StationNameGeneratorPull = {"Montparnasse", "St Jean", "Du Sud", "De l'Ouest",
        "Austerlitz", "Stalingrad", "Lavoisier" };
    [SerializeField]int nameIndex = 0; //private
    [SerializeField] public int NameLooped = 1; //private

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
        //if (Input.GetMouseButton(0))
        //gameUI.DoSelection();
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
        string stationName = StationNameGeneratorPull[nameIndex];
        nameIndex += 1;
        if (nameIndex > StationNameGeneratorPull.Length - 1)
        {
            nameIndex = 0;
            NameLooped += 1;
        }
        return stationName;
    }
}
