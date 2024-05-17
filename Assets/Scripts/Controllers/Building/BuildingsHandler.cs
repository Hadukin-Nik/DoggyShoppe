using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsHandler
{
    //Here I have ALL BUILDED structures with there placement dots in matrix
    private List<(BuildingContoller, List<(int, int)>)> _buildings;


    public BuildingsHandler()
    {
        _buildings = new List<(BuildingContoller, List<(int, int)>)>();
    }

    public (BuildingContoller, List<(int, int)>) getRandom()
    {
        return _buildings[Random.Range(0, _buildings.Count)];
    }

    public void Add(BuildingContoller buildingContoller, List<(int, int)> dotsInMatrix)
    {
        _buildings.Add((buildingContoller, dotsInMatrix));
    }

    public List<(BuildingContoller, List<(int, int)>)> GetAll()
    {
        return _buildings;
    }
}
