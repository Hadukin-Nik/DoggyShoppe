using System.Collections.Generic;
using UnityEngine;

public class BuildingsHandler
{
    //Here I have ALL BUILDED structures with there placement dots in matrix
    private HashSetNListStructure<ItemHolder> _buildingStructure;

    public BuildingsHandler()
    {
        _buildingStructure = new HashSetNListStructure<ItemHolder>();
    }

    public List<(int, int)> GetRandom()
    {
        return _buildingStructure.GetRandom().getPoints();
    }

    public void Add(ItemHolder buildingContoller)
    {
        _buildingStructure.Add(buildingContoller);
    }

    public List<ItemHolder> GetAll()
    {
        return _buildingStructure.GetAll();
    }

    public ItemHolder getRandom()
    {
        return _buildingStructure.GetRandom();
    }
}
