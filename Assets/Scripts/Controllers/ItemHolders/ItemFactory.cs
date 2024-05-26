using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{

    [SerializeField] private List<Item> items;
    public void Add(Item item)
    {
        items.Add(item);
    }

    public void Remove(int index) 
    { 
        items.RemoveAt(index);
    }

    public Item Get(ItemsConsts.ItemIndificator itemIndificator)
    {
        int index = getIndex(itemIndificator);
        if (index != -1)
        {
            return items[index];
        } else
        {
            return null;
        }
    }

    private int getIndex(ItemsConsts.ItemIndificator itemIndificator)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (items[i]._itemIndificator == itemIndificator)
            {
                return i;
            }
        }
        return -1;
    }
}
