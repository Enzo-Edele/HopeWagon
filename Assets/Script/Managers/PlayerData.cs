using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

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
        
    }
    private void FixedUpdate()
    {
        if (gameTimer < 0)
        {
            //GameManager.Instance.saveLoadMenu.EndGameDemo(completeContract);
        }
        else if (gameTimer >= 0)
        {
            gameTimer += Time.deltaTime;
            GameManager.Instance.pauseMenu.TimerUpdate((int)gameTimer);
        }
    }
    private void LoadSTartGame()
    {
        railStock = 50;
        stationStock = 7;
        trainStock = 7;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
        //GameManager.Instance.pauseMenu.ActivateMenu();//for demo
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

    public void Save(BinaryWriter writer)
    {
        writer.Write((int)gameTimer);
        writer.Write(railStock);
        writer.Write(stationStock);
        writer.Write(trainStock);

        for(int i = 0; i < contratPool.Count; i++)
        {
            writer.Write(contratPool[i].contract.id);
            if(i < 3)
            {
                writer.Write(contratPool[i].requiredQty);
                writer.Write(contratPool[i].accumulated);
            }
        }

        writer.Write(completeContract);
    }
    public void Load(BinaryReader reader, int header)
    {
        if(header >= 2)
        {
            gameTimer = reader.ReadInt32();
            railStock = reader.ReadInt32();
            stationStock = reader.ReadInt32();
            trainStock = reader.ReadInt32();
            GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
        }
        if(header >= 3)
        {
            for (int i = 0; i < contratPool.Count; i++)
            {
                contratPool[i].SetType(GameManager.Instance.contratTypes[reader.ReadInt32()]);
                if (i < 3)
                {
                    contratPool[i].requiredQty = reader.ReadInt32();
                    contratPool[i].accumulated = reader.ReadInt32();
                }
            }
            GameManager.Instance.gameUI.updateContractDisplay(contratPool);
        }
        if (header >= 4)
            completeContract = reader.ReadInt32();
    }

    public void Cheat()
    {
        memRail = railStock;
        memStation = stationStock;
        memTrain = trainStock;
        railStock = 999;
        stationStock = 999;
        trainStock = 999;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
    }
    public void StopCheat()
    {
        railStock = memRail;
        stationStock = memStation;
        trainStock = memTrain;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
    }

    //debug and reset fct

    public void ResetRessource()
    {
        railStock = 50;
        stationStock = 7;
        trainStock = 7;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
    }
}
