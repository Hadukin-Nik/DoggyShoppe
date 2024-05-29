using System.Collections.Generic;
using UnityEngine;

public class BuildingsFactory : MonoBehaviour
{
    [SerializeField] private List<Building> buildings;


    public void Add(Building building)
    {
        buildings.Add(building);
    }

    public void Remove(int index)
    {
        buildings.RemoveAt(index);
    }

    public Building Get(BuildingsConsts.BuildingIndificator buildingIndificator)
    {
        int index = getIndex(buildingIndificator);
        if (index != -1)
        {
            BuildingContoller bc = buildings[index]._gameBody.GetComponent<BuildingContoller>();

            if (bc == null) {
                buildings[index]._gameBody.AddComponent<BuildingContoller>();
                bc = buildings[index]._gameBody.GetComponent<BuildingContoller>();

                bc.SetData(new Building(buildingIndificator, buildings[index]._gameBody, buildings[index]._name));
            }
            return buildings[index];
        }
        else
        {
            return null;
        }
    }

    private int getIndex(BuildingsConsts.BuildingIndificator buildingIndificator)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i]._buildingIndificator == buildingIndificator)
            {
                return i;
            }
        }
        return -1;
    }
}
