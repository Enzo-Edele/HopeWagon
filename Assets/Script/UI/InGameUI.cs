using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InGameUI : MonoBehaviour
{
    public GridBoard grid;

    GameTile currentTile;

    //Unit

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            /*if (Input.GetMouseButton(0))
                DoSelection();
            else if (selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                    //DoMove();
                else
                    //DoPathFinding();
            }*/
        }
    }

    bool UpdateCurrentTile()
    {
        //GameTile tile = grid.GetTile(Input.mousePosition);
        GameTile tile = GameManager.Instance.selectedTile;
        if (!currentTile) currentTile = tile; currentTile.Paint(Color.green);
        if (currentTile && tile != currentTile)
        {
            currentTile.Paint(Color.white);
            currentTile = tile;
            currentTile.Paint(Color.green);
            return true;
        }
        return false;
    }

    public void DoSelection()
    {
        //grid.ClearPath();
        //UpdateCurrentTile();
        //if (currentTile)
        //selectedUnit = currentTile.Unit;
    }
}
