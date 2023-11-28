using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIndustry", menuName = "Industry ScriptableObject")]

public class IndustryScriptable : ScriptableObject
{
    public string nameIndustry;
    public int id;
    public RessourceScriptable outpout; // list
    public int prodTime;
    public int prodQty; //list
    public RessourceScriptable input; // list
    public int requireAmount; //list
    public int storage; //???
    //for later use
    public int pollutionLevel;
    public string description;
    //IndustyScriptable upgrade;
}
