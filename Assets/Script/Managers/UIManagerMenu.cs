using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class TutoLabel
{
    public List<GameObject> list;
}

public class UIManagerMenu : MonoBehaviour
{
    public GridBoard gridBoard;

    const int mapFileVersion = 3;

    [SerializeField] GameObject pausePanel;
    bool menuIsActive = false;

    //add tutorial button
    [SerializeField] GameObject ecopediaPanel;
    [SerializeField] List<GameObject> tutoPanels;
    [SerializeField] List<TutoLabel> TutoCategories;
    int currentTutoLabel = 0;
    int currentTutoPannel = 0;
    [SerializeField] TMP_Text tipsIndicator;
    //tutorial open at begining
    [SerializeField] GameObject endPanel;
    [SerializeField] TMP_Text endTextContract;
    [SerializeField] TMP_Text endTextTime;

    [SerializeField] TMP_Text timerText;

    [SerializeField] GameObject helpButton;

    public void Save(string path) {
        GameManager.Instance.mainMenuUI.ButtonOpenSaveLoadMenu(true);
        GameManager.Instance.gameUI.ActivateInGameUI(false);
        menuIsActive = !menuIsActive;
        pausePanel.SetActive(menuIsActive);
        /*
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(mapFileVersion);
            gridBoard.Save(writer);
            GameManager.Instance.playerData.Save(writer);
        }*/
    }

    public void Load(string path)
    {
        GameManager.Instance.mainMenuUI.ButtonOpenSaveLoadMenu(false);
        GameManager.Instance.gameUI.ActivateInGameUI(false);
        menuIsActive = !menuIsActive;
        pausePanel.SetActive(menuIsActive);
        /*
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
                gridBoard.Load(reader, header);
                GameManager.Instance.playerData.StopCheat();
                if (header >= 2)
                    GameManager.Instance.playerData.Load(reader, header);
                //GridBoard.Instance.RefreshPollutionMax();
            }
            else
                Debug.LogWarning("Unkwown map format " + header);
        }
        */
    }

    public void HelpButton(int indexHelp)
    {
        if (!menuIsActive)
        {
            menuIsActive = true;
            ecopediaPanel.SetActive(true);
            currentTutoPannel = indexHelp;
            TutoCategories[currentTutoLabel].list[currentTutoPannel].SetActive(true);
            tipsIndicator.text = (currentTutoPannel + 1) + " / " + (TutoCategories[currentTutoLabel].list.Count);
        }
    }
    public void ActivateMenu()
    {
        menuIsActive = !menuIsActive;
        pausePanel.SetActive(menuIsActive);
        //pause and unpause game
        if (menuIsActive)
            GameManager.Instance.ChangeGameState(GameManager.GameState.pause);
        else
            GameManager.Instance.ChangeGameState(GameManager.GameState.inGame);
    }

    public void ActivateHelpButton(bool nState)
    {
        helpButton.SetActive(nState);
    }
    public void Resume()
    {
        ActivateMenu();
    }
    public void Restart()
    {
        endPanel.SetActive(false);
        pausePanel.SetActive(false);
        SceneManager.LoadScene("Main");
    }
    public void Continue()
    {
        Time.timeScale = 1.0f;
        //GameManager.Instance.playerData.SetTimer(9999);
        endPanel.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }

    //Demo
    //use bouton to make tuto label
    //use list of image to choose right advice to display
    //use list of list for each label and their advices
    public void SelectTuto(int part)
    {
        TutoCategories[currentTutoLabel].list[currentTutoPannel].SetActive(false);
        currentTutoPannel = 0;
        currentTutoLabel = part;
        TutoCategories[currentTutoLabel].list[currentTutoPannel].SetActive(true);
        tipsIndicator.text = (currentTutoPannel + 1) + " / " + (TutoCategories[currentTutoLabel].list.Count);
    }
    public void ActivateTuto(bool newState)
    {
        ecopediaPanel.SetActive(newState);
        TutoCategories[currentTutoLabel].list[currentTutoPannel].SetActive(newState);
        pausePanel.SetActive(!newState);
        tipsIndicator.text = (currentTutoPannel + 1) + " / " + (TutoCategories[currentTutoLabel].list.Count);
    }

    public void NextTuto()
    {
        TutoCategories[currentTutoLabel].list[currentTutoPannel].SetActive(false);
        currentTutoPannel++;
        if (currentTutoPannel >= TutoCategories[currentTutoLabel].list.Count)
            currentTutoPannel = 0;
        TutoCategories[currentTutoLabel].list[currentTutoPannel].SetActive(true);
        tipsIndicator.text = (currentTutoPannel + 1) + " / " + (TutoCategories[currentTutoLabel].list.Count);
    }
    public void PreviousTuto()
    {
        TutoCategories[currentTutoLabel].list[currentTutoPannel].SetActive(false);
        currentTutoPannel--;
        if (currentTutoPannel < 0)
            currentTutoPannel = TutoCategories[currentTutoLabel].list.Count - 1;
        TutoCategories[currentTutoLabel].list[currentTutoPannel].SetActive(true);
        tipsIndicator.text = (currentTutoPannel + 1) + " / " + (TutoCategories[currentTutoLabel].list.Count);
    }

    public void TimerUpdate(int time)
    {
        timerText.text = time + "s";
    }
    public void EndGameDemo(int completeContract, float timer)
    {
        endPanel.SetActive(true);
        endTextContract.text = "Contract Completed : " + completeContract;
        endTextTime.text = "Time Played : " + (int)timer;
        Time.timeScale = 0.0f;
    }
}
