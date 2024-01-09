using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Industry : MonoBehaviour
{
    int prodTime = 3;
    float prodTimer;
    List<int> prodQty; 
    List<int> requiredQty; 

    List<RessourceScriptable> ressourceInput = new List<RessourceScriptable>();
    List<RessourceScriptable> ressourceOutput = new List<RessourceScriptable>();

    [SerializeField] public List<int> exportID; //{ get; private set; }
    [SerializeField] public List<int> importID; //{ get; private set; }
    [SerializeField] public List<bool> canExport; //{ get; private set; }
    [SerializeField] public List<bool> canImport; //{ get; private set; }

    [SerializeField] int storage = 10; //
    [SerializeField] public List<int> stockRessources; //{ get; private set; }
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
                industryName.text = value.nameIndustry;
                //do the next part later
                model.GetComponent<Renderer>().material = GameManager.Instance.Industrymats[value.id];
            }
        }
    }

    [SerializeField] Canvas canvas;
    [SerializeField] TMP_Text industryName;
    [SerializeField] Transform outputDisplay;
    [SerializeField] Transform inputDisplay;
    [SerializeField] GameObject imagePrefab;
    //exportsIcon

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
        exportID = new List<int>();
        importID = new List<int>();
    }
    private void Start() {
        prodTimer = prodTime;
        stockRessources = new List<int>();
        for(int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++)
        {
            stockRessources.Add(0);
            canExport.Add(false);
            canImport.Add(false);
        }
    }
    public void SetIndustryType(IndustryScriptable newType) {
        Type = newType;
        prodTimer = prodTime;
        SetAcceptedRessources(ressourceInput, importID, canImport, inputDisplay);
        SetAcceptedRessources(ressourceOutput, exportID, canExport, outputDisplay);
        for(int i = 0; i < linkedStation.Count; i++) {
            linkedStation[i].CheckImportExport();
        }
    }
    void SetAcceptedRessources(List<RessourceScriptable> listToCheck, List<int> listToUpdate, List<bool> checkToUpdate, Transform display)
    {
        listToUpdate.Clear();
        checkToUpdate.Clear();
        for(int j = 0; j < GameManager.Instance.ressourceTypes.Count; j++) {
            checkToUpdate.Add(false);
        }
        for(int k = 1; k < display.childCount; k++)
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
                Image icon = Instantiate(imagePrefab, display).GetComponent<Image>();
                icon.sprite = GameManager.Instance.ressourceTypes[i].sprite;
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
            bool canProd = CanProduce();
            if (importID.Count >= 0) {
                if (canProd) {
                    for (int i = 0; i < importID.Count; i++)
                        ChangeStorageRessource(-requiredQty[i], importID[i]);
                    for (int i = 0; i < exportID.Count; i++)
                        ChangeStorageRessource(prodQty[i], exportID[i]); 
                    prodTimer = prodTime;
                }
                prodTimer = 1;
            }
            if(importID.Count < 0) {
                for (int i = importID.Count; i < exportID.Count; i++)
                    ChangeStorageRessource(prodQty[i], exportID[i]); 
                prodTimer = prodTime;
            }
        }
        else if(prodTimer >= 0 ) {
            prodTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        canvas.transform.rotation = Camera.main.transform.rotation;
    }

    bool CanProduce()
    {
        for(int i = 0; i < importID.Count; i++) {
            if (stockRessources[importID[i]] < requiredQty[i])
                return false;
        }
        return true;
    }

    //use to transfer ressource if return is < 0 there isn't enough ressources don't proceed further
    //                                    is > 0 ther isn't enough storage return the ecxess
    public int ChangeStorageRessource(int changeValue, int valueIndex)
    {
        int leftover = 0;
        stockRessources[valueIndex] += changeValue;
        if (stockRessources[valueIndex] < 0) {
            leftover = stockRessources[valueIndex];
            stockRessources[valueIndex] = 0;
        }
        else if (stockRessources[valueIndex] > storage) {
            leftover = stockRessources[valueIndex] - storage;
            stockRessources[valueIndex] = storage;
        }

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
    }
}
