using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIndustry", menuName = "Industry ScriptableObject")]

public class IndustryScriptable : ScriptableObject
{
    public string nameIndustry;
    public int id;
    public Material mat;
    public List<RessourceScriptable> outpout;
    public int prodTime;
    public List<int> prodQty;
    public List<RessourceScriptable> input;
    public List<int> requireAmount;
    public List<int> storage; //??? List
    //for later use
    public float pollutionLevel;
    public string description;
    //IndustyScriptable upgrade;
}
