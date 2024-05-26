using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HashSetNListStructure<T>
{
    private List<T> _list;
    private Dictionary<T, int> _itemIndex;

    public HashSetNListStructure() {
        _list = new List<T>();
        _itemIndex = new Dictionary<T, int>();  
    }

    public T GetRandom()
    {
        int randomIndex = Random.Range(0, _list.Count);
        return _list[randomIndex];
    }
    public List<T> GetAll()
    {
        return _list;
    }
    public void Add(T item)
    {
        _itemIndex.Add(item, _list.Count);
        _list.Add(item);
    }

    public void Remove(T item)
    {
        if(!_itemIndex.ContainsKey(item)) {
            Console.Error.WriteLine("WRONG ACTION IN SETNLIST, THERE ARE NO REMOVING ITEMS");
            return;
        }

        int index = _itemIndex[item];
        T last = _list[_list.Count - 1];

        _itemIndex[last] = index;
        _itemIndex.Remove(item);

        _list[index] = last;
        _list.RemoveAt(_list.Count - 1);
    }

   public int Size()
   {
        return _list.Count;
   }
}
