using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionManager : MonoBehaviour
{
    float updateTime = 0.5f;
    float updateTimer = 0f;

    [Range(0f, 1f)]
    public float groundSpreadFactor = 0.5f;

    [Range(0f, 1f)]
    public float pollutionDecay = 0.0f;

    [System.Serializable]
    struct PollutionData
    {
        public float groundPollution, airPollution;
    }
    [SerializeField] List<PollutionData> pollution = new List<PollutionData>();
    [SerializeField] List<PollutionData> nextPollution = new List<PollutionData>();

    public void Init()
    {
        CreatePollution();
        updateTimer = updateTime;
    }

    void CreatePollution()
    {
        pollution.Clear();
        nextPollution.Clear();
        PollutionData initialData = new PollutionData();
        initialData.groundPollution = 0;
        PollutionData clearData = new PollutionData();
        for (int i = 0; i < GridBoard.Instance.tileCount; i++)
        {
            pollution.Add(initialData);
            nextPollution.Add(clearData);
        }
        UpdatePollution();
    }

    private void Update()
    {
        if (updateTimer > 0)
            updateTimer -= Time.deltaTime;
        else if (updateTimer < 0)
        {
            //UpdatePollution();
            updateTimer = updateTime;
        }
    }

    void UpdatePollution()
    {
        for (int i = 0; i < GridBoard.Instance.tileCount; i++) {
            NewEvolvePollution(i);
        }
        List<PollutionData> swap = pollution;
        pollution = nextPollution;
        nextPollution = swap;
    }
    void NewEvolvePollution(int tileIndex)
    {
        bool interract = false;
        GameTile tile = GridBoard.Instance.GetTile(tileIndex);
        PollutionData tilePollution = pollution[tileIndex];

        if (tile.HasIndustry)
        {
            tilePollution.groundPollution = tile.industry.pollutionLevel;
            interract = true;
        }
        else if (tile.HasPollutedIndustry)
        {
            tilePollution.groundPollution = tile.pollutedIndustry.pollutionLevel;
            interract = true;
        }
        else if (tile.pollutionLevel != 0.0f) {
            tilePollution.groundPollution = tile.pollutionLevel;
            interract = true; 
        }

        float groundRunoff = tilePollution.groundPollution * groundSpreadFactor * (1f / 4f);
        //int maxPollution = 1;
        int minPollution = -10;

        if (interract)
        {
            Debug.Log(groundRunoff);
            for(int i = 0; i < 4; i++)
            {
                GameTile neighbor = tile.neighbor[i];
                if (!neighbor)
                    continue;
                PollutionData neighborPollution = nextPollution[neighbor.index];

                neighbor.SetPollutionLevel(neighbor.pollutionLevel + (int)groundRunoff);
                //Or use a factor determine by industry type

                if (neighbor.pollutionLevel > tile.pollutionLevel)
                    neighbor.SetPollutionLevel(tile.pollutionLevel);
                if (neighbor.pollutionLevel < minPollution)
                    neighbor.SetPollutionLevel(minPollution);

                nextPollution[neighbor.index] = neighborPollution;
            }

            PollutionData nextTilePollution = nextPollution[tileIndex];
            //insert cap pollution here
            nextPollution[tileIndex] = nextTilePollution;
            pollution[tileIndex] = new PollutionData();

            tile.SetPollutionLevel((int)tilePollution.groundPollution);
        }
    }
    void EvolvePollution(int tileIndex)
    {
        GameTile tile = GridBoard.Instance.GetTile(tileIndex);
        PollutionData tilePollution = pollution[tileIndex];

        //set level of pollutedcell
        if (tile.HasIndustry)
        {
            tilePollution.groundPollution = tile.industry.pollutionLevel;
        }
        if (tile.HasPollutedIndustry)
        {
            tilePollution.groundPollution = tile.pollutedIndustry.pollutionLevel;
        }
        else
        {
            tilePollution.groundPollution -= pollutionDecay;
        }
        if (tilePollution.groundPollution < 0)
            tilePollution.groundPollution = 0.0f;

        float groundRunoff = tilePollution.groundPollution * groundSpreadFactor * (1f / 4f);

        int maxPollution = 0;

        for(int i = 0; i < 4; i++)
        {
            GameTile neighbor = tile.neighbor[i];
            if (!neighbor)
                continue;
            PollutionData neighborPollution = nextPollution[neighbor.index];

            neighborPollution.groundPollution += groundRunoff;

            //set max pollution
            if (tile.neighbor[i].pollutionLevel > maxPollution)
                maxPollution = (int)tile.neighbor[i].pollutionLevel;

            nextPollution[neighbor.index] = neighborPollution;
        }
        if (maxPollution < 1)
            maxPollution = 1;

        PollutionData nextTilePollution = nextPollution[tileIndex];
        //cap value here
        if (nextTilePollution.groundPollution > maxPollution - 1)
            nextTilePollution.groundPollution = maxPollution - 1;
        nextPollution[tileIndex] = nextTilePollution;
        pollution[tileIndex] = new PollutionData();

        tile.SetPollutionLevel((int)tilePollution.groundPollution);
    }
}
