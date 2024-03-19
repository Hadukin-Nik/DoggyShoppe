using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private ItemsConsts.ItemIndificator itemIndificator;

    [SerializeField] private int _countOfItems;

    public int CountOfItems()
    {
        return _countOfItems;
    }

    public int grabItems(int itemCount)
    {
        if(itemCount < _countOfItems)
        {
            _countOfItems -= itemCount;
        } else {
            itemCount = _countOfItems;
            _countOfItems = 0;
        }

        return itemCount;
    }
}
