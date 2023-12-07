using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int railStock;
    public int stationStock;
    public int trainStock;

    //list des types de contrat
    [SerializeField]GameObject contractPrefab;
    [SerializeField]List<ContractScriptable> contractsType;
    public List<Contract> contratPool;
    public List<int> contractOfTypeGiven;
    //contrats pool //pool rigged with the 6~9 first being forced on easy and 1 out of 3 granted to be an easy one;

    public static GameManager Instance { get; private set; }

    void Start()
    {
        railStock = 999; //50
        stationStock = 20; //10
        trainStock = 50; //5
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
        GameManager.Instance.gameUI.updateContractDisplay(contratPool);
        LoadSTartGame(); //use for demo build
    }
    private void LoadSTartGame()
    {
        railStock = 50;
        stationStock = 10;
        trainStock = 5;
        GameManager.Instance.gameUI.UpdatePlayerData(railStock, stationStock, trainStock);
    }
    public void ChangeRailStock(int qty)
    {
        railStock += qty;
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
            }
        }
        Contract newContract = Instantiate(contractPrefab, this.transform).GetComponent<Contract>();
        newContract.Randomize();
        contratPool.Add(newContract);
        GameManager.Instance.gameUI.updateContractDisplay(contratPool);
        return foundContract;
    }
}
