using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveLoadMenu : MonoBehaviour
{
    public GridBoard gridBoard;

    const int mapFileVersion = 0;

    [SerializeField] GameObject menuPanel;
    [SerializeField] TMP_Text resumeText;
    bool menuIsActive = false;

    //add tutorial button
    [SerializeField] GameObject tutoPanel;
    [SerializeField] List<GameObject> tutoPanels;
    int currentTutoPannel = 0;
    [SerializeField] TMP_Text tipsIndicator;
    //tutorial open at begining
    [SerializeField] GameObject endPanel;
    [SerializeField] TMP_Text endText;

    [SerializeField] TMP_Text timerText;

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
        if (menuIsActive)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }
    public void Resume()
    {
        ActivateMenu();
        resumeText.text = "Resume";
    }
    public void Restart()
    {
        endPanel.SetActive(false);
        menuPanel.SetActive(false);
        SceneManager.LoadScene("Main");
    }
    public void Continue()
    {
        Time.timeScale = 1.0f;
        GameManager.Instance.playerData.SetTimer(9999);
        endPanel.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }

    //Demo
    public void ActivateTuto(bool newState)
    {
        tutoPanel.SetActive(newState);
        tutoPanels[currentTutoPannel].SetActive(newState);
        menuPanel.SetActive(!newState);
        tipsIndicator.text = (currentTutoPannel + 1) + " / " + (tutoPanels.Count);
    }

    public void NextTuto()
    {
        tutoPanels[currentTutoPannel].SetActive(false);
        currentTutoPannel++;
        if (currentTutoPannel >= tutoPanels.Count)
            currentTutoPannel = 0;
        tutoPanels[currentTutoPannel].SetActive(true);
        tipsIndicator.text = (currentTutoPannel + 1) + " / " + (tutoPanels.Count);
    }
    public void PreviousTuto()
    {
        tutoPanels[currentTutoPannel].SetActive(false);
        currentTutoPannel--;
        if (currentTutoPannel < 0)
            currentTutoPannel = tutoPanels.Count - 1;
        tutoPanels[currentTutoPannel].SetActive(true);
        tipsIndicator.text = (currentTutoPannel + 1) + " / " + (tutoPanels.Count);
    }

    public void TimerUpdate(int time)
    {
        timerText.text = time + "s";
    }
    public void EndGameDemo(int completeContract)
    {
        endPanel.SetActive(true);
        endText.text = "Contract Completed : " + completeContract;
        Time.timeScale = 0.0f;
    }
}
