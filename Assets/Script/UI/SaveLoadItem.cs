using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveLoadItem : MonoBehaviour
{
    public SaveLoadMenu menu;
    //set name display on the button
    public string MapName
    {
        get { return mapName; }
        set
        {
            mapName = value;
            transform.GetChild(0).GetComponent<TMP_Text>().text = value;
        }
    }
    string mapName;
    //fct called when clicking on the button
    public void Select()
    {
        menu.SelectItem(mapName);
    }
}
