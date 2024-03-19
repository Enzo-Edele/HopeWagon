using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManagerMainMenu : MonoBehaviour
{
    #region Declaration
    [SerializeField] GameObject mainMenu;

    [SerializeField] GameObject saveMenu;
    [SerializeField] TMP_Text saveMenuTitle;
    [SerializeField] TMP_Text saveMenuActionLabel;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] RectTransform fileList;
    [SerializeField] SaveLoadItem fileItemPrefab;
    bool saveMode;

    [SerializeField] GameObject optionMenu;

    const int mapFileVersion = 3;
    #endregion

    public static UIManagerMainMenu Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    
    public void ButtonNewGame()
    {
        mainMenu.SetActive(false);
        GameManager.Instance.ChangeGameState(GameManager.GameState.inGame);
        GameManager.Instance.gameUI.ActivateInGameUI(true);
        GameManager.Instance.gameUI.OpenTutoMenu(true);
    }

    //save system
    public void ButtonOpenSaveLoadMenu(bool saveMode)
    {
        this.saveMode = saveMode;
        if (saveMode)
        {
            saveMenuTitle.text = "Save Map";
            saveMenuActionLabel.text = "Save";
        }
        else
        {
            saveMenuTitle.text = "Load Map";
            saveMenuActionLabel.text = "Load";
        }
        FillList();
        saveMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    void FillList()
    {
        for (int i = 0; i < fileList.childCount; i++)
            Destroy(fileList.GetChild(i).gameObject);
        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
        Array.Sort(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(fileItemPrefab);
            item.menu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(fileList, false);
        }
    }
    string GetSelectPath()
    {
        string mapName = inputField.text;
        if (mapName.Length == 0)
            return null;
        return Path.Combine(Application.persistentDataPath, mapName + ".map");
        //to avoid wrong character limit the one allowed by the input component
    }
    public void SelectItem(string name)
    {
        inputField.text = name;
    }
    public void Action()
    {
        string path = GetSelectPath();
        if (path == null)
            return;
        if (saveMode)
            Save(path);
        else
            Load(path);
        
        saveMenu.SetActive(false);
        GameManager.Instance.gameUI.ActivateInGameUI(true);

        GameManager.Instance.ChangeGameState(GameManager.GameState.inGame);
    }
    public void Delete()
    {
        string path = GetSelectPath();
        if (path == null)
            return;
        if (File.Exists(path))
            File.Delete(path);
        inputField.text = "";
        FillList();
    }
    public void Save(string path)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(mapFileVersion);
            GameManager.Instance.gridBoard.Save(writer);
            GameManager.Instance.playerData.Save(writer);
        }
    }

    public void Load(string path)
    {
        GameManager.Instance.playerData.Cheat();
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
                GameManager.Instance.gridBoard.Load(reader, header);
                //load TrainRoute
                GameManager.Instance.playerData.StopCheat();
                if (header >= 2)
                    GameManager.Instance.playerData.Load(reader, header);
                //GridBoard.Instance.RefreshPollutionMax();
            }
            else
                Debug.LogWarning("Unkwown map format " + header);
        }
    }
    public void ButtonCloseSaveLoadMainMenu()
    {
        saveMenu.SetActive(false);
        if (GameManager.gameState == GameManager.GameState.pause)
        {
            GameManager.Instance.ChangeGameState(GameManager.GameState.inGame);
            GameManager.Instance.gameUI.ActivateInGameUI(true);
        }
        else
        {
            mainMenu.SetActive(true);
        }
    }

    public void ButtonOption()
    {
        optionMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
