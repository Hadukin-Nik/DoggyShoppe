using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] private string _mainPointTag;
    [SerializeField] private Vector3 _size;
    [SerializeField] private Transform _pivot;

    private ItemFactory _factory;

    private List<(int, int)> _points;

    private Item _itemPrefab;
    
    private Stack<GameObject> _itemStack;

    private int _maxCount;
    private int _free;

    private void Start()
    {
        GameObject mainPoint = GameObject.FindWithTag(_mainPointTag);
        _factory = mainPoint.GetComponent<ItemFactory>();
        _itemStack = new Stack<GameObject>();

        setPlaceEmpty();
    }

    private void setPlaceEmpty()
    {
        _itemPrefab = _factory.Get(ItemsConsts.ItemIndificator.Empty);
        _maxCount = 0;
    }


    public bool IsItemPlaceable(ItemsConsts.ItemIndificator toPlace)
    {
        
        bool e =  (_itemPrefab._itemIndificator == toPlace || _itemPrefab._itemIndificator == ItemsConsts.ItemIndificator.Empty) && (_maxCount == 0 || _maxCount > _itemStack.Count);
        return e;
    }

    public int getFreeItems()
    {
        return _free;
    }

    public void setFreeItems(int free)
    {
        _free = free;
    }

    public void AddNewItem(ItemsConsts.ItemIndificator toPlace)
    {
        if(_itemPrefab._itemIndificator == ItemsConsts.ItemIndificator.Empty)
        {
            _itemPrefab = _factory.Get(toPlace);
            _maxCount = (int)(_size.x / _itemPrefab._size.x) * (int)(_size.z / _itemPrefab._size.z);
        } 

        placeItem();
    }

    public void DestroyLastItem() {
        if(_itemStack.Count == 0)
        {
            Console.Error.WriteLine("Guests trying get items, that arent in stock");
        }
        GameObject item = _itemStack.Pop();

        Destroy(item);

        if(_itemStack.Count == 0)
        {
            setPlaceEmpty();
        }
    }

    public List<(int, int)> getPoints()
    {
        if(_points == null)
        {
            Console.Error.WriteLine("ERROR IN ITEM HOLDER: there are no set points");
        }

        return _points;
    }
    public void setPoints(List<(int, int)> points)
    {
        _points = points;
    }


    private void placeItem()
    {
        _free++;
        
        float deltaZ = (_itemStack.Count / (int)(_size.x / _itemPrefab._size.x)) * _itemPrefab._size.z;
        float deltaX = (_itemStack.Count % (int)(_size.x / _itemPrefab._size.x)) * _itemPrefab._size.x;

        Vector3 nv = (_itemPrefab._size.x / 2 + deltaX) * _pivot.right + (_itemPrefab._size.z / 2 + deltaZ) * _pivot.forward;

        float x = nv.x + _pivot.position.x;
        float z = nv.z + _pivot.position.z;
        Quaternion rotation = transform.rotation;
        rotation.x += _itemPrefab._gameBody.transform.rotation.x;
        rotation.y += _itemPrefab._gameBody.transform.rotation.y;
        rotation.z += _itemPrefab._gameBody.transform.rotation.z;
        _itemStack.Push(Instantiate(_itemPrefab._gameBody, new Vector3(x, transform.position.y, z), rotation));


    }

}
