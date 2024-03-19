using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PollutedIndustry : MonoBehaviour
{
    List<RessourceScriptable> ressourceInput = new List<RessourceScriptable>();
    [SerializeField] List<int> requiredQty = new List<int>();
    public IndustryScriptable IndustryScriptable;
    public float pollutionLevel;

    [SerializeField] List<int> acquiredQty = new List<int>();

    [SerializeField] public List<int> exportID; //{ get; private set; }
    [SerializeField] public List<int> importID; //{ get; private set; }
    [SerializeField] public List<bool> canExport; //{ get; private set; }
    [SerializeField] public List<bool> canImport; //{ get; private set; }

    PollutedScriptable type;
    public PollutedScriptable Type
    {
        get { return type; }
        set {
            if (type != value) {
                type = value;
                ressourceInput = value.input;
                requiredQty = value.requireAmount;
                acquiredQty.Clear();
                for (int i = 0; i < requiredQty.Count; i++)
                    acquiredQty.Add(0);
                IndustryScriptable = value.industryScriptable;

                pollutionLevel = value.pollutionLevel;
                name = value.nameBuilding;
                industryName.text = value.nameBuilding;
                model.GetComponent<Renderer>().material = value.mat;
            }
        }
    }

    GameTile tile;
    public void SetTile(GameTile ownerTile)
    {
        tile = ownerTile;
    }
    List<Station> linkedStation = new List<Station>();

    [SerializeField] GameObject model;
    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text industryName;
    [SerializeField] Transform requireDisplay;
    [SerializeField] GameObject requireItem;
    List<RouteRessourceItem> itemsUI = new List<RouteRessourceItem>();

    private void Awake()
    {
        exportID = new List<int>();
        importID = new List<int>();
    }

    private void LateUpdate()
    {
        canvas.transform.rotation = Camera.main.transform.rotation;
    }

    public void SetIndustryType(PollutedScriptable newType)
    {
        Type = newType;
        SetAcceptedRessources(ressourceInput, importID, canImport, requireDisplay);
        for (int i = 0; i < linkedStation.Count; i++)
        {
            linkedStation[i].CheckImportExport();
        }
        tile.SetMaxPollution((int)pollutionLevel);
    }

    public void AddStation(Station industry)
    {
        for (int i = 0; i < linkedStation.Count; i++)
            if (linkedStation[i] == industry)
                return;
        linkedStation.Add(industry);
    }
    public void RemoveStation(Station industry)
    {
        for (int i = 0; i < linkedStation.Count; i++)
            if (industry == linkedStation[i])
                linkedStation.RemoveAt(i);
    }

    void SetAcceptedRessources(List<RessourceScriptable> listToCheck, List<int> listToUpdate, List<bool> checkToUpdate, Transform display)
    {
        listToUpdate.Clear();
        checkToUpdate.Clear();
        itemsUI.Clear();
        for (int j = 0; j < GameManager.Instance.ressourceTypes.Count; j++)
        {
            checkToUpdate.Add(false);
        }
        for (int k = 1; k < display.childCount; k++)
            Destroy(display.GetChild(k).gameObject);
        int i = 0;
        int toFind = listToCheck.Count;
        int found = 0;
        while (i < GameManager.Instance.ressourceTypes.Count && found < toFind)
        {
            if (GameManager.Instance.ressourceTypes[i].id == listToCheck[found].id)
            {
                listToUpdate.Add(listToCheck[found].id);
                checkToUpdate[i] = true;
                RouteRessourceItem item = Instantiate(requireItem, display).GetComponent<RouteRessourceItem>();
                item.Set(listToCheck[found].id, requiredQty[found]);
                itemsUI.Add(item);
                i = -1;
                found++;
            }
            i++;
        }
    }

    public int AddRessource(int ressourceID, int amount)
    {
        int leftover = 0;
        for (int i = 0; i < ressourceInput.Count; i++) {
            if (ressourceID == ressourceInput[i].id) {
                acquiredQty[i] += amount;
                //Debug.Log("I = " + i);
            }
        }
        for (int i = 0; i < requiredQty.Count; i++) {
            if (acquiredQty[i] > requiredQty[i]) {
                leftover = acquiredQty[i] - requiredQty[i];
                acquiredQty[i] = requiredQty[i];
            }
        }
        bool canDepollute = true;
        for (int i = 0; i < requiredQty.Count; i++) {
            if (!(acquiredQty[i] >= requiredQty[i])) {
                canDepollute = false;
            }
        }
        if (canDepollute)
            Depollute();

        UpdateUI();
        
        return leftover;
    }

    public void Depollute()
    {
        //call tile function to upgrade industry
        tile.UpgradePolluted(type.id);
        if(type.id == 8)
        {
            Debug.Log("call win");
            GameManager.Instance.pauseMenu.EndGameDemo(GameManager.Instance.playerData.completeContract, GameManager.Instance.playerData.gameTimer);
        }
    }

    public void UpdateUI()
    {
        for(int i = 0; i < itemsUI.Count; i++)
        {
            itemsUI[i].UpdatePolluted(ressourceInput[i].id, acquiredQty[i], requiredQty[i]);
        }
    }
    public void ShowHideUI(bool state)
    {
        canvas.gameObject.SetActive(state);
    }
}
