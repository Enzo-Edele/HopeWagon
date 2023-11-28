using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Industry : MonoBehaviour
{
    //string ressourceName = "ressource1";
    int prodTime = 3;
    float prodTimer;
    int prodQty = 2;

    [SerializeField] int storage = 10;
    [SerializeField] List<int> starageRessources = new List<int>();
    [SerializeField] public int stock { get; private set; }
    [SerializeField] List<int> stockRessources = new List<int>();
    public GameObject model;

    //List<Station> linkedStation = new List<Station>();

    RessourceScriptable ressourceInput; //to list
    RessourceScriptable ressourceOutput; //to list
    int required; //to list
    public IndustryScriptable type { 
        get { return type; }
        set {
            ressourceOutput = value.outpout;
            prodTime = value.prodTime;
            prodQty = value.prodQty;
            ressourceInput = value.input;
            required = value.requireAmount;
            //do the next part later
        }
    }

    //toscrap
    [SerializeField] TMP_Text stockDisplay;
    [SerializeField] TMP_Text requiredStockDisplay;

    private void Start()
    {
        prodTimer = prodTime;
        stock = 0;
    }
    void Update()
    {
        if(prodTimer < 0) {
            prodTimer = prodTime;
            ChangeStorage(prodQty);
        }
        else {
            prodTimer -= Time.deltaTime;
        }
        //call ressource consumption
    }
    //use to transfer ressource if return is < 0 there isn't enough ressources don't proceed further
    //                                    is > 0 ther isn't enough storage return the ecxess
    public int ChangeStorage(int changeValue)
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
    }
    public int ChangeStorageRessources(int changeValue, int valueIndex)
    {
        int leftover = 0; 

        stockRessources[valueIndex] += changeValue;//changer en liste
        if (stockRessources[valueIndex] < 0)
        {
            leftover = stockRessources[valueIndex];
            stockRessources[valueIndex] = 0;
        }
        else if (stockRessources[valueIndex] > storage)
        {
            leftover = stockRessources[valueIndex] - storage;
            stockRessources[valueIndex] = storage;
        }
        stockDisplay.text = "" + stockRessources[valueIndex];

        return leftover;
    }
    public int ChangeMultipleStorage(List<int> changeValue, List<int> valueIndex)
    {
        int leftover = 0; //changer en liste

        //inStock += changeValue; //créer list in stock
        //loop pour chaque changement
        if (stock < 0)
        {
            leftover = stock;
            stock = 0;
        }
        else if (stock > storage)
        {
            leftover = stock - storage;
            stock = storage;
        }
        stockDisplay.text = "" + stock;

        return leftover;
    }
}
