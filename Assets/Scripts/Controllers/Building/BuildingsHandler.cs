using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsHandler
{
    //Here I have ALL BUILDED buildings with there placement dots in matrix
    private List<KeyValuePair<BuildingContoller, List<KeyValuePair<int, int>>>>_buildings;
    
    public BuildingsHandler()
    {
        _buildings = new List<KeyValuePair<BuildingContoller, List<KeyValuePair<int, int>>>>();
    }

    public KeyValuePair<BuildingContoller, List<KeyValuePair<int, int>>> getRandom()
    {
        return _buildings[Random.Range(0, _buildings.Count)];
    }

    public void Add(BuildingContoller buildingContoller, List<KeyValuePair<int, int>> dotsInMatrix)
    {
        _buildings.Add(new KeyValuePair<BuildingContoller, List<KeyValuePair<int, int>>>(buildingContoller, dotsInMatrix));
    }

    public List<KeyValuePair<BuildingContoller, List<KeyValuePair<int, int>>>> GetAll()
    {
        return _buildings;
    }
}
