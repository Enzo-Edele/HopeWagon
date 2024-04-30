using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contract : MonoBehaviour
{
    public ContractScriptable contract;

    public int requireRessourcesIndex;
    public int requiredQty = 0;
    public string requiredDisplayText
    {
        get { return accumulated + " / " + requiredQty; }
        private set { }
    }
    public string reward;
    public Sprite rewardIcon;
    public int rewardQty = 0;
    public string rewardDisplayText
    {
        get { return "" + rewardQty; }
        private set { }
    }
    public int accumulated = 0;

    //referencer dans le playerdata
    //call d'une fonction qui vérifie

    void Start()
    {
        if (contract != null)
        {
            requireRessourcesIndex = contract.requiredRessource.id;
            requiredQty = contract.requiredQty;
            reward = contract.reward;
            rewardQty = contract.rewardQty;
            rewardIcon = contract.rewardIcon;
            GameManager.Instance.playerData.contractOfTypeGiven[contract.id]++;
            name = contract.nameContract + " " + GameManager.Instance.playerData.contractOfTypeGiven[contract.id];
            GameManager.Instance.gameUI.updateContractDisplay(GameManager.Instance.playerData.contratPool);
        }
    }

    public void Randomize()
    {
        contract = GameManager.Instance.contratTypes[Random.Range(0, GameManager.Instance.contratTypes.Count)];
        requireRessourcesIndex = contract.requiredRessource.id;
        requiredQty = contract.requiredQty;
        reward = contract.reward;
        rewardQty = contract.rewardQty;
        rewardIcon = contract.rewardIcon;
        GameManager.Instance.playerData.contractOfTypeGiven[contract.id]++;
        name = contract.nameContract + " " + GameManager.Instance.playerData.contractOfTypeGiven[contract.id];
    }

    public void SetType(ContractScriptable type)
    {
        contract = type;
        requireRessourcesIndex = contract.requiredRessource.id;
        requiredQty = contract.requiredQty;
        reward = contract.reward;
        rewardQty = contract.rewardQty;
        rewardIcon = contract.rewardIcon;
        GameManager.Instance.playerData.contractOfTypeGiven[contract.id]++;
        name = contract.nameContract + " " + GameManager.Instance.playerData.contractOfTypeGiven[contract.id];
    }

    public void AddToObjective(int amout)
    {
        accumulated += amout;
        if(accumulated >= requiredQty) {
            GiveReward();
        }
        GameManager.Instance.gameUI.updateContractDisplay(GameManager.Instance.playerData.contratPool);
    }

    void GiveReward()
    {
        switch (reward)
        {
            case "train":
            {
                GameManager.Instance.playerData.ChangeTrainStock(rewardQty);
                break;
            }
            case "station":
            {
                GameManager.Instance.playerData.ChangeStationStock(rewardQty);
                break;
            }
            case "rail":
            {
                GameManager.Instance.playerData.ChangeRailStock(rewardQty);
                break;
            }
        }
        GameManager.Instance.playerData.CompleteContract(this.gameObject);
        Destroy(gameObject);
    }
}
