using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Industry : MonoBehaviour
{
    //string ressourceName = "ressource1";
    int prodTime = 3;
    float prodTimer;
    List<int> prodQty; //to list
    List<int> requiredQty; //to list

    List<RessourceScriptable> ressourceInput = new List<RessourceScriptable>();
    List<RessourceScriptable> ressourceOutput = new List<RessourceScriptable>();

    [SerializeField] public List<int> canExport;//{ get; private set; }
    [SerializeField] public List<int> canImport;//{ get; private set; }

    [SerializeField] int storage = 10; //
    [SerializeField] public int stock { get; private set; } //
    [SerializeField] public List<int> stockRessources;//{ get; private set; } //update pour avour un stockage sur All ressource slot
    public GameObject model;

    List<Station> linkedStation = new List<Station>();

    IndustryScriptable type;
    public IndustryScriptable Type { 
        get { return type; }
        set {
            if(type != value) { 
                type = value;
                ressourceOutput = value.outpout;
                prodTime = value.prodTime;
                prodQty = value.prodQty;
                ressourceInput = value.input;
                requiredQty = value.requireAmount;
                //do the next part later
                model.GetComponent<Renderer>().material = GameManager.Instance.Industrymats[value.id];
            }
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

    private void Awake()
    {
        canExport = new List<int>();
        canImport = new List<int>();
    }
    private void Start() {
        prodTimer = prodTime;
        stockRessources = new List<int>();
        for(int i = 0; i < GameManager.Instance.ressourceSample.Count; i++)
        {
            stockRessources.Add(0);
        }
    }
    public void SetIndustryType(IndustryScriptable newType) {
        Type = newType;
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
        //update la production que la c'est pas bon pas bon DU TOUT
        if(prodTimer < 0) {
            prodTimer = prodTime;
            bool canProd = CanProduce();
            if (canImport.Count >= 0) {
                if (canProd) {
                    for (int i = 0; i < canImport.Count; i++)
                        ChangeStorageRessource(-requiredQty[i], canImport[i]);
                    for (int i = 0; i < canExport.Count; i++)
                        ChangeStorageRessource(prodQty[i], canExport[i]); //to update when prod qty will become a list
                    prodTimer = prodTime;
                }
                prodTimer = 1;
            }
            if(canImport.Count < 0) {
                for (int i = canImport.Count; i < canExport.Count; i++)
                    ChangeStorageRessource(prodQty[i], canExport[i]); //to update when prod qty will become a list
                prodTimer = prodTime;
            }
        }
        else if(prodTimer >= 0 ) {
            prodTimer -= Time.deltaTime;
        }
    }

    bool CanProduce()
    {
        for(int i = 0; i < canImport.Count; i++) {
            if (stockRessources[canImport[i]] < requiredQty[i])
                return false;
        }
        return true;
    }

    //use to transfer ressource if return is < 0 there isn't enough ressources don't proceed further
    //                                    is > 0 ther isn't enough storage return the ecxess
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
