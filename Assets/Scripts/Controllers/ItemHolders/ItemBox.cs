using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private ItemsConsts.ItemIndificator _itemIndificator;

    [SerializeField] private int _countOfItems;

    [SerializeField] private Vector3 _size = new Vector3 (1,1,1);

    private List<(int, int)> _usedPoints;
    private BoxStorage _boxStorage;

    private (int, int) wherePlaced;

    public int CountOfItems()
    {
        return _countOfItems;
    }

    public void SetCountOfItems(int items)
    {
        _countOfItems = items;
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
        return _itemIndificator;
    }

    public List<(int, int)> GetUsedPoints()
    {
        return _usedPoints;
    }
    public (int, int) WherePlaced() { return wherePlaced; }
    public void SetUsedPoints(List<(int, int)> usedPoints)
    {
        _usedPoints = usedPoints;
    }

    public void SetStorage(BoxStorage boxStorage) { 
        _boxStorage = boxStorage;
    }

    public BoxStorage getBoxStorage()
    {
        return _boxStorage;
    }

    public Vector3 GetSize() { return _size; }

    public void SetWherePlaced((int, int) item3)
    {
        wherePlaced = item3;
    }

    public void SetItem(ItemsConsts.ItemIndificator itemIndificator)
    {
        _itemIndificator = itemIndificator;
    }
}
