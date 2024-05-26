using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlaceableObject {
    public GameObject _gameBody;
    public Vector3 _size;
    public string _name;

    public PlaceableObject() { }

    public PlaceableObject(GameObject gameBody, string name)
    {
        _gameBody = gameBody;   
        _name = name;   
    }

    public GameObject _getGameObject { get { return _gameBody; } }
    public void setGameObject(GameObject gameBody)
    {
        this._gameBody = gameBody;
    }

    public void setSize(Vector3 size) {
        _size = size;
    }

    public string getName() { return _name; }
    public void setName(string name)
    {
        this._name = name;
    }
}
