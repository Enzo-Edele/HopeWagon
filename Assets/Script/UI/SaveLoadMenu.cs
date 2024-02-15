using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveLoadMenu : MonoBehaviour
{
    public TMP_Text menuLabel, actionButtonLabel;

    public InputField nameInput;

    public RectTransform listContent;
    public SaveLoadItem itemPrefab;

    public GridBoard gridBoard;

    bool saveMode;

    const int mapFileVersion = 5;
    //fct for saveLoadMenu button
    public void Open(bool saveMode)
    {
        this.saveMode = saveMode;
        if (saveMode)
        {
            menuLabel.text = "Save Map";
            actionButtonLabel.text = "Save";
        }
        else
        {
            menuLabel.text = "Load Map";
            actionButtonLabel.text = "Load";
        }
        FillList();
        gameObject.SetActive(true);
        //HexMapCamera.Locked = true;
    }
    //fct to close saveLoadMenu
    public void Close()
    {
        gameObject.SetActive(false);
        //HexMapCamera.Locked = false;
    }
    //get the name in the input bar
    string GetSelectPath()
    {
        string mapName = nameInput.text;
        if (mapName.Length == 0)
            return null;
        return Path.Combine(Application.persistentDataPath, mapName + ".map");
        //to avoid wrong character limit the one allowed by the input component
    }
    //save current map under the name in the input bar
    public void Save(string path)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(mapFileVersion);
            gridBoard.Save(writer);
        }
    }
    //load selected map
    public void Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.Log("File does not exist " + path);
            return;
        }
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();
            if (header <= mapFileVersion)
            {
                gridBoard.Load(reader, header);
                //HexMapCamera.ValidatePosition();
            }
            else
                Debug.LogWarning("Unkwown map format " + header);
        }
    }
    //call save or load depending on the menu open
    public void Action()
    {
        string path = GetSelectPath();
        if (path == null)
            return;
        if (saveMode)
            Save(path);
        else
            Load(path);
        Close();
    }
    //store the name of the currently selected map
    public void SelectItem(string name)
    {
        nameInput.text = name;
    }
    //fill the list of map with all saved map
    void FillList()
    {
        for (int i = 0; i < listContent.childCount; i++)
            Destroy(listContent.GetChild(i).gameObject);
        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
        Array.Sort(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(itemPrefab);
            item.menu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(listContent, false);
        }
    }
    //delete selected map
    public void Delete()
    {
        string path = GetSelectPath();
        if (path == null)
            return;
        if (File.Exists(path))
            File.Delete(path);
        nameInput.text = "";
        FillList();
    }
}
