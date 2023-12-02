using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Industry : MonoBehaviour
{
    //string ressourceName = "ressource1";
    int prodTime = 3;
    float prodTimer;
    int prodQty = 2; //to list
    int requiredQty; //to list

    List<RessourceScriptable> ressourceInput = new List<RessourceScriptable>();
    List<RessourceScriptable> ressourceOutput = new List<RessourceScriptable>();

    public List<int> canExport { get; private set; }
    public List<int> canImport { get; private set; }

    [SerializeField] int storage = 10; //
    //[SerializeField] List<int> storageRessources = new List<int>();
    [SerializeField] public int stock { get; private set; } //
    [SerializeField] public List<int> stockRessources { get; private set; } //update pour avour un stockage sur All ressource slot
    [SerializeField] int[,] stockRessourcesArray = new int[0, 0];
    public GameObject model;

    List<Station> linkedStation = new List<Station>();

    public IndustryScriptable type { 
        get { return type; }
        set {
            ressourceOutput = value.outpout;
            prodTime = value.prodTime;
            prodQty = value.prodQty;
            ressourceInput = value.input;
            requiredQty = value.requireAmount;
            //do the next part later
        }
    }

    //toscrap
    [SerializeField] TMP_Text stockDisplay;
    [SerializeField] TMP_Text requiredStockDisplay;

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

    //ajouter une fonction pour update UI when selected
    private void Start() {
        prodTimer = prodTime;
        canExport = new List<int>();
        canImport = new List<int>();
        stockRessources = new List<int>();
        for(int i = 0; i < GameManager.Instance.ressourceSample.Count; i++)
        {
            stockRessources.Add(0);
        }
    }
    public void SetIndustryType(IndustryScriptable newType) {
        type = newType;
        prodTimer = prodTime;
        SetAcceptedRessources(ressourceInput, canImport);
        SetAcceptedRessources(ressourceOutput, canExport);
        for(int i = 0; i < linkedStation.Count; i++) {
            linkedStation[i].CheckImportExport();
        }
    }
    void SetAcceptedRessources(List<RessourceScriptable> listToCheck, List<int> listToUpdate)
    {
        listToUpdate.Clear();
        int i = 0;
        int toFind = listToCheck.Count;
        int found = 0;
        while (i < GameManager.Instance.ressourceSample.Count && found < toFind)
        {
            if (GameManager.Instance.ressourceSample[i].id == listToCheck[found].id)
            {
                listToUpdate.Add(listToCheck[found].id);
                i = -1;
                found++;
            }
            i++;
        }
    }

    void Update()
    {
        if(prodTimer < 0) {
            prodTimer = prodTime;
            for(int j = 0; j < canImport.Count; j++) { 
                if (CanProduce(canImport[j])) { 
                    for (int i = 0; i < canImport.Count; i++)
                        ChangeStorageRessource(-requiredQty, canImport[j]); //to update when require qty will become a list
                    for (int i = canImport.Count; i < canImport.Count + canExport.Count; i++)
                        ChangeStorageRessource(prodQty, canImport[j]); //to update when prod qty will become a list
                    prodTimer = prodTime;
                }
            }
        }
        else if(prodTimer >= 0 ) {
            prodTimer -= Time.deltaTime;
        }
    }

    bool CanProduce(int ressourceIndex)
    {
        for(int i = 0; i < canImport.Count; i++)
            if (requiredQty < stockRessources[ressourceIndex])
                return false;
        return true;
    }

    //use to transfer ressource if return is < 0 there isn't enough ressources don't proceed further
    //                                    is > 0 ther isn't enough storage return the ecxess
    /*public int ChangeStorage(int changeValue)
    {
        int leftover = 0;

        stock += changeValue;
        if (stock < 0) { 
            leftover = stock;
            stock = 0;
        }
        else if (stock > storage) {
            leftover = stock - storage;
            stock = storage;
        }
        stockDisplay.text = "" + stock;

        return leftover;
    }
    public void SetStock(int newStock)
    {
        stock = newStock;
        if (stock < 0)
            stock = 0;
        else if (stock > storage)
            stock = storage;
        stockDisplay.text = "" + stock;
    }*/
    public int ChangeStorageRessource(int changeValue, int valueIndex)
    {
        int leftover = 0;
        //au besoin récupérer les id depuis l'index de la liste
        stockRessources[valueIndex] += changeValue;
        if (stockRessources[valueIndex] < 0) {
            leftover = stockRessources[valueIndex];
            stockRessources[valueIndex] = 0;
        }
        else if (stockRessources[valueIndex] > storage) {
            leftover = stockRessources[valueIndex] - storage;
            stockRessources[valueIndex] = storage;
        }

        //Update UI string

        return leftover;
    }
    public void SetStockRessource(int newStock, int valueIndex)
    {
        Debug.Log("station change stock id : " + valueIndex + " stock : " + newStock);
        //ajouter sécurité pour voir si ressource utilisé
        stockRessources[valueIndex] = newStock;
        if (stockRessources[valueIndex] < 0)
            stockRessources[valueIndex] = 0;
        else if (stockRessources[valueIndex] > storage)
            stockRessources[valueIndex] = storage;

        //vérifier si leftover ???

        //Update UI string
    }

    //not needed now
    public List<int> ChangeMultipleStorage(List<int> changeValue, List<int> valueIndex)
    {
        List<int> leftover = new List<int>(); //changer en liste

        //inStock += changeValue; //créer list in stock
        //loop pour chaque changement
        if (stock < 0)
        {
            //leftover = stock;
            stock = 0;
        }
        else if (stock > storage)
        {
            //leftover = stock - storage;
            stock = storage;
        }
        stockDisplay.text = "" + stock;

        return leftover;
    }
}
