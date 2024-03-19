using System.Collections;
using System.Collections.Generic;
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
}
