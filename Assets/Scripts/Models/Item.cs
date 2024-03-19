using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {
    public ItemsConsts.ItemIndificator _itemIndificator;
    public GameObject _gameBody;
    public string _name;

    public Item() { }

    public Item(ItemsConsts.ItemIndificator itemIndificator, GameObject gameBody, string name)
    {
        _itemIndificator = itemIndificator;
        _gameBody = gameBody;   
        _name = name;   
    }

    public ItemsConsts.ItemIndificator getItemIndificator { get { return _itemIndificator;} }
    public void setItemIndificator(ItemsConsts.ItemIndificator itemIndificator)
    {
        this._itemIndificator = itemIndificator;
    }

    public GameObject _getGameObject { get { return _gameBody; } }
    public void setGameObject(GameObject gameBody)
    {
        this._gameBody = gameBody;
    }

    public string getName() { return _name; }
    public void setName(string name)
    {
        this._name = name;
    }
}
