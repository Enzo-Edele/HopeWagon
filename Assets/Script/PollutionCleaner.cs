using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionCleaner : MonoBehaviour
{
    public float pollutionLevel;

    [SerializeField] List<RessourceScriptable> ressourceInput = new List<RessourceScriptable>();

    [SerializeField] public List<int> importID; //{ get; private set; }
    [SerializeField] public List<bool> canImport; //{ get; private set; }

    [SerializeField] List<int> storage = new List<int>();
    [SerializeField] List<Vector2Int> storageCheck = new List<Vector2Int>();
    [SerializeField] public List<int> stockRessources; //{ get; private set; }

    List<Station> linkedStation = new List<Station>();

    GameTile tile;
    public void SetTile(GameTile ownerTile)
    {
        tile = ownerTile;
        SetUpCleaner();
    }

    public void AddStation(Station industry)
    {
        for (int i = 0; i < linkedStation.Count; i++)
            if (linkedStation[i] == industry)
                return;
        linkedStation.Add(industry);
    }

    private void Awake()
    {
        importID = new List<int>();
        stockRessources = new List<int>();
        for (int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++) {
            stockRessources.Add(0);
            canImport.Add(false);
        }
    }

    public void SetUpCleaner()
    {
        SetAcceptedRessources(ressourceInput, importID, canImport);
        for (int i = 0; i < linkedStation.Count; i++)
        {
            linkedStation[i].CheckImportExport();
        }
        tile.LowerMinPollution((int)pollutionLevel);
    }

    void SetAcceptedRessources(List<RessourceScriptable> listToCheck, List<int> listToUpdate, List<bool> checkToUpdate)
    {
        listToUpdate.Clear();
        checkToUpdate.Clear();
        for (int j = 0; j < GameManager.Instance.ressourceTypes.Count; j++)
        {
            checkToUpdate.Add(false);
        }
        int i = 0;
        int toFind = listToCheck.Count;
        int found = 0;
        while (i < GameManager.Instance.ressourceTypes.Count && found < toFind)
        {
            if (GameManager.Instance.ressourceTypes[i].id == listToCheck[found].id)
            {
                listToUpdate.Add(listToCheck[found].id);
                checkToUpdate[i] = true;
                i = -1;
                found++;
            }
            i++;
        }
    }

    public int ChangeStorageRessource(int changeValue, int valueIndex)
    {
        int leftover = 0;
        stockRessources[valueIndex] += changeValue;
        if (stockRessources[valueIndex] < 0)
        {
            leftover = stockRessources[valueIndex];
            stockRessources[valueIndex] = 0;
        }
        else
        {
            for (int i = 0; i < storageCheck.Count; i++)
            {
                if (valueIndex == storageCheck[i].x)
                {
                    if (stockRessources[valueIndex] > storageCheck[i].y)
                    {
                        leftover = stockRessources[valueIndex] - storage[i];
                        stockRessources[valueIndex] = storage[i];
                    }
                }
            }
        }

        return leftover;
    }
}
