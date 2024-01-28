using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    public int railStock;
    public int stationStock;
    public int trainStock;
    int memRail, memStation, memTrain;

    //list des types de contrat
    [SerializeField]GameObject contractPrefab;
    [SerializeField]List<ContractScriptable> contractsType;
    public List<Contract> contratPool;
    public List<int> contractOfTypeGiven;
    //contrats pool //pool rigged with the 6~9 first being forced on easy and 1 out of 3 granted to be an easy one;

    public float gameTimer = 0.0f;
    int gameTime = 0;
    public int completeContract = 0;

    public static GameManager Instance { get; private set; }

    void Start()
    {
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
        GameManager.Instance.gameUI.updateContractDisplay(contratPool);
        LoadSTartGame(); //use for demo build
        gameTimer = gameTime;
    }
    private void Update()
    {
        if(gameTimer < 0)
        {
            //GameManager.Instance.saveLoadMenu.EndGameDemo(completeContract);
        }
        else if(gameTimer >= 0)
        {
            gameTimer += Time.deltaTime;
            GameManager.Instance.saveLoadMenu.TimerUpdate((int)gameTimer);
        }
    }
    private void LoadSTartGame()
    {
        railStock = 50;
        stationStock = 7;
        trainStock = 7;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
        GameManager.Instance.saveLoadMenu.ActivateMenu();//for demo
    }
    public void ChangeRailStock(int qty)
    {
        railStock += qty;
        if (railStock < 0)
            railStock = 0;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
    }
    public void ChangeStationStock(int qty)
    {
        stationStock += qty;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
    }
    public void ChangeTrainStock(int qty)
    {
        trainStock += qty;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
    }

    public void AddContratProgress(int ressourceIndex, int qty) //to list
    {
        for (int i = 0; i < 3; i++)
        {
            if (contratPool[i].requireRessourcesIndex == ressourceIndex)  //error
            {
                contratPool[i].AddToObjective(qty);
                //Debug.Log("" + GameManager.Instance.ressourceTypes[ressourceIndex].name + " ressource added equal : " + qty);
            }
        }
        GameManager.Instance.gameUI.updateContractDisplay(contratPool);
    }

    public bool CompleteContract(GameObject contract)
    {
        bool foundContract = false;
        for (int i = 0; i < 3; i++) {
            if (contratPool[i].name == contract.name) {
                contratPool.RemoveAt(i);
                foundContract = true;
                completeContract++;
            }
        }
        Contract newContract = Instantiate(contractPrefab, this.transform).GetComponent<Contract>();
        newContract.Randomize();
        contratPool.Add(newContract);
        GameManager.Instance.gameUI.updateContractDisplay(contratPool);
        return foundContract;
    }
    public void SetTimer(int time)
    {
        gameTimer = time;
    }

    public void Cheat()
    {
        memRail = railStock;
        memStation = stationStock;
        memTrain = trainStock;
        railStock = 999;
        stationStock = 999;
        trainStock = 999;
    }
    public void StopCheat()
    {
        railStock = memRail;
        stationStock = memStation;
        trainStock = memTrain;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
    }
}
