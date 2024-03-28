using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutoManager : MonoBehaviour
{
    [SerializeField] TMP_Text tutoText;
    [SerializeField] Button nextTutoButton;
    [SerializeField] TMP_Text nextTutoButtonText;
    [SerializeField] Button closeTutoButton;
    [SerializeField] TMP_Text closeTutoButtonText;

    [SerializeField] List<string> tutoAdvices = new List<string>();

    [SerializeField] GameObject stationIndicatorPrefab;
    [SerializeField] GameObject railIndicatorPrefab;
    List<GameObject> visualObject = new List<GameObject>();

    int stationPlaced = 0;
    int railPlaced = 0;

    public int tutoIndex { get; private set; }
    public static TutoManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        tutoIndex = -1;
    }

    //to remove
    private void Start()
    {
        NextTutoButton();
    }

    void ChangeTutoText(int tutoID)
    {
        tutoText.text = tutoAdvices[tutoID];
    }
    public void ChangeTutoTextButton(string text)
    {
        nextTutoButtonText.text = text;
    }
    public void ChangeCloseTutoTextButton(string text)
    {
        closeTutoButtonText.text = text;
    }

    public void NextTutoButton()
    {
        switch (tutoIndex)
        {
            case -1:
                //ask player if starting tutorial
                NextAdvice(tutoIndex);
                ChangeTutoTextButton("Yes");
                ChangeCloseTutoTextButton("No");
                closeTutoButton.gameObject.SetActive(true);
                break;
            case 0:
                //show first tutorial advice
                NextAdvice(tutoIndex);
                ChangeTutoTextButton("Suivant");
                closeTutoButton.gameObject.SetActive(false);
                break;
            case 1:
                //teach how to place station
                nextTutoButton.interactable = false;
                NextAdvice(tutoIndex);
                GameManager.Instance.cameraController.LockCamOnPos(new Vector2(-4.5f, -18.0f));
                GameManager.Instance.cameraController.LockCamHeight(5.0f);
                GameManager.Instance.cameraController.LockCam(true);
                GameManager.Instance.gameUI.SpawnTutoStation();
                //ds in game ui fct tuto qui check a la pose des gare pour rendre interactible le bouton
                break;
            case 2:
                //teach how to place rail
                nextTutoButton.interactable = false;
                NextAdvice(tutoIndex);
                GameManager.Instance.gameUI.SpawnTutoRail();
                //ds in game ui fct tuto qui check a la pose des rails pour rendre interactible le bouton
                break;
            case 3:
                //teach to create route
                nextTutoButton.interactable = false;
                NextAdvice(tutoIndex);
                GameManager.Instance.cameraController.LockCam(false);
                //ds in game ui ?pannel spécial avec bouton unique pour lancement de route?
                break;
            case 4:
                //teach how station menu work 1/2
                NextAdvice(tutoIndex);
                break;
            case 5:
                //teach how station menu work 2/2
                NextAdvice(tutoIndex);
                break;
            case 6:
                //intro limiting factor
                NextAdvice(tutoIndex);
                break;
            case 7:
                //teach player ressources
                NextAdvice(tutoIndex);
                GameManager.Instance.gameUI.ActivateOutlinerRessource(true);
                break;
            case 8:
                //teach contract
                NextAdvice(tutoIndex);
                GameManager.Instance.gameUI.ActivateOutlinerRessource(false);
                GameManager.Instance.gameUI.ActivateOutlinerContract(true);
                break;
            case 9:
                //teach contract replenishment
                NextAdvice(tutoIndex);
                GameManager.Instance.gameUI.ActivateOutlinerContract(false);
                break;
            case 10:
                //teach objective
                NextAdvice(tutoIndex);
                GameManager.Instance.cameraController.LockCamOnPos(new Vector2(4.5f, -9.5f));
                GameManager.Instance.cameraController.LockCamHeight(4.0f);
                GameManager.Instance.cameraController.LockCam(true);
                break;
            case 11:
                GameManager.Instance.ChangeGameState(GameManager.GameState.inGame);
                CloseTuto();
                break;
        }
    }
    void NextAdvice(int id)
    {
        tutoIndex = id + 1;
        ChangeTutoText(tutoIndex);
    }
    public void TutoStepDone()
    {
        nextTutoButton.interactable = true;
        ClearVisualPrefab();
        if(tutoIndex != 4 && tutoIndex != 5)
        GameManager.Instance.gameUI.Unselect();
    }

    public void PlaceStationIndicator(Vector2 coordinate)
    {
        GameObject tile = GameManager.Instance.gridBoard.GetTile(new Vector3(coordinate.x, 0, coordinate.y)).gameObject;
        GameObject prefab = Instantiate(stationIndicatorPrefab, tile.transform);
        visualObject.Add(prefab);
    }
    public void PlaceRailIndicator(Vector2 coordinate)
    {
        GameObject tile = GameManager.Instance.gridBoard.GetTile(new Vector3(coordinate.x, 0, coordinate.y)).gameObject;
        GameObject prefab = Instantiate(railIndicatorPrefab, tile.transform);
        visualObject.Add(prefab);
    }

    public void StationPlaced()
    {
        stationPlaced++;
        if (stationPlaced >= 2)
            TutoStepDone();
    }
    public void RailPlaced()
    {
        railPlaced++;
        if (railPlaced >= 3)
            TutoStepDone();
    }
    void ClearVisualPrefab()
    {
        for(int i = 0; i < visualObject.Count; i++)
        {
            Destroy(visualObject[i]);
        }
        visualObject.Clear();
    }

    public void NextTutoButtonPressed()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.tuto);
        NextTutoButton();
    }
    public void CloseTuto()
    {
        GameManager.Instance.gameUI.OpenTutoMenu(false);
        GameManager.Instance.cameraController.LockCam(false);
    }
}
