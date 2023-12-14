using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contract : MonoBehaviour
{
    [SerializeField]ContractScriptable contract;

    public int requireRessourcesIndex;
    public int required = 0;
    public string reward;
    public int rewardQty = 0;
    public int accumulated = 0;

    //referencer dans le playerdata
    //call d'une fonction qui vérifie

    void Start()
    {
        if (contract != null)
        {
            requireRessourcesIndex = contract.requiredRessource.id;
            required = contract.requiredQty;
            reward = contract.reward;
            rewardQty = contract.rewardQty;
            GameManager.Instance.gameUI.updateContractDisplay(GameManager.Instance.playerData.contratPool);
            GameManager.Instance.playerData.contractOfTypeGiven[contract.id]++;
            name = contract.nameContract + " " + GameManager.Instance.playerData.contractOfTypeGiven[contract.id];
        }
    }

    public void Randomize()
    {
        contract = GameManager.Instance.contratTypes[Random.Range(0, GameManager.Instance.contratTypes.Count)];
        requireRessourcesIndex = contract.requiredRessource.id;
        required = contract.requiredQty;
        reward = contract.reward;
        rewardQty = contract.rewardQty;
        GameManager.Instance.playerData.contractOfTypeGiven[contract.id]++;
        name = contract.nameContract + " " + GameManager.Instance.playerData.contractOfTypeGiven[contract.id];
    }

    public void AddToObjective(int amout)
    {
        accumulated += amout;
        if(accumulated >= required) {
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
                    Debug.Log("gain : " + rewardQty + " train");
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
