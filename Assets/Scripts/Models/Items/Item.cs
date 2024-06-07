using UnityEngine;


[System.Serializable]
public class Item : PlaceableObject {
    public ItemsConsts.ItemIndificator _itemIndificator;
    public Material _boxMaterial;
    public Item() { }

    public Item(ItemsConsts.ItemIndificator itemIndificator, GameObject gameBody, string name)
    {
        
        _itemIndificator = itemIndificator;
        _gameBody = gameBody;   
        _name = name;   
    }

    public ItemsConsts.ItemIndificator getItemIndificator() { return _itemIndificator; }
    public void setItemIndificator(ItemsConsts.ItemIndificator itemIndificator) { _itemIndificator = itemIndificator; }
    public Material getBoxMaterial() { return _boxMaterial;}
}
