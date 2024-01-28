using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRessource", menuName = "Ressource ScriptableObject")]

public class RessourceScriptable : ScriptableObject
{
    public string nameRessource;
    public int id;
    public int tier;
    public int transportScore;
    public Sprite sprite;
    public Material mat;
    //for later use

    //type de transport
}
