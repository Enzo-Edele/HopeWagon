using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveLoadMenu : MonoBehaviour
{
    public GridBoard gridBoard;

    const int mapFileVersion = 0;

    [SerializeField] GameObject menuPanel;
    bool menuIsActive = false;

    //add tutorial button
    //tutorial open at begining
    //make a panel list and a forward backward button

    public void Save(string path) {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(mapFileVersion);
            gridBoard.Save(writer);
        }
    }

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
                //setCameraPos
            }
            else
                Debug.LogWarning("Unkwown map format " + header);
        }
    }

    public void ActivateMenu()
    {
        menuIsActive = !menuIsActive;
        menuPanel.SetActive(menuIsActive);
        //pause and unpause game
    }

    public void Restart()
    {
        SceneManager.LoadScene("Main");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
