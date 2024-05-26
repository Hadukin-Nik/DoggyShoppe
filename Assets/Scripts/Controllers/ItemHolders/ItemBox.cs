using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private ItemsConsts.ItemIndificator itemIndificator;

    [SerializeField] private int _countOfItems;

    private List<(int, int)> _usedPoints;

    public int CountOfItems()
    {
        return _countOfItems;
    }

    public bool isAnyItemIn()
    {
        return _countOfItems >= 1;
    }

    public bool tryDecreaseAmount()
    {
        return --_countOfItems >= 0 ? true : (++_countOfItems) > 0;
    }

    public bool tryIncreaseAmount()
    {
        return ++_countOfItems >= 0 ? true : true;
    }

    public ItemsConsts.ItemIndificator GetItemIndificator()
    {
        return itemIndificator;
    }

    public List<(int, int)> GetUsedPoints()
    {
        return _usedPoints;
    }

    public void SetUsedPoints(List<(int, int)> usedPoints)
    {
        _usedPoints = usedPoints;
    }
}
