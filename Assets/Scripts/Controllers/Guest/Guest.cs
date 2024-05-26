using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Guest : MonoBehaviour
{
    [SerializeField] private float _height;
    [SerializeField] private float _timeWait;
    private PauseMenuController _menuController;
    private EconomyController _priceMenuController;
    private Floor _floorController;
    private Queue<(ItemHolder, int)> _items;
    private MoveGuest _mover;
    private float _time;
    private bool _waiting;

    private Action _onStartAction;

    private int _moneyForShopping = 0;
    void Start()
    {
        _menuController = FindAnyObjectByType<PauseMenuController>();
        _floorController = FindAnyObjectByType<Floor>();
        _priceMenuController = FindAnyObjectByType<EconomyController>();  
         _mover = transform.GetComponent<MoveGuest>();
        if (_floorController == null)
        {
            Debug.Log("Cannot find floor controller on scene");
        }
        _waiting = true;
        _mover.SetActionOnEach(ReleaseNextItem);
        _mover.SetActionOnEnd(OnCashAction);
        _mover.SetHeight(_height);
        _time = _timeWait;
    }

    // Update is called once per frame
    void Update()
    {
        if(_time <= 0f && _waiting)
        {
            _waiting = false;
            Vector3 point = transform.position;

            List<Vector3> points = new List<Vector3>();
            Queue<int> itemHolders = new Queue<int>();
            int cashIndex = 0;
            List<(ItemHolder, int)> holdersItems = new List<(ItemHolder, int)> ();
            HashSet<ItemHolder> visited = new HashSet<ItemHolder>();
            int lengthWay = UnityEngine.Random.Range(1, 4);
            _items = new Queue<(ItemHolder, int)>();
            
            for(int l = 0; l < lengthWay; l++)
            {
                
                (List<Vector3>, ItemHolder) points2 = _floorController.GetWayToRandom(point);
                ItemHolder item = points2.Item2;

                if (item == null || item.getFreeItems() == 0 || visited.Contains(item))
                {
                    continue;
                }
                
                visited.Add(item);
                points.AddRange(points2.Item1);
                point = points[points.Count - 1];
                itemHolders.Enqueue(points.Count - 1);
                int use = UnityEngine.Random.RandomRange(1, Math.Max(1, Math.Min(item.getFreeItems(), 6)));
                _items.Enqueue((item, use));
                holdersItems.Add((item, item.getFreeItems() - use));
            }

            if(_items.Count == 0)
            {
                return;
            }

            for(int i = 0; i < 5; i++)
            {
                List<Vector3> points2 = _floorController.GetWayToRandomEndPoint(point);

                if (points2 != null)
                {
                    points.AddRange(points2);
                    point = points[points.Count - 1];
                    cashIndex = points.Count - 1;
                    break;
                }

                if(points2 == null && i == 4)
                {
                    return;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                List<Vector3> points2 = _floorController.GetWayToExitPoint(point);

                if (points2 != null)
                {
                    points.AddRange(points2);
                    point = points[points.Count - 1];
                    break;
                }

                if (points2 == null && i == 4)
                {
                    return;
                }
            }
            foreach(var i in holdersItems)
            {
                i.Item1.setFreeItems(i.Item2);
            }
            _mover.Move(points, itemHolders, cashIndex);
            _onStartAction();
        }

        _time -= Time.deltaTime;
    }

    private void setEnableToMove()
    {
        if(_timeWait == -1f) { return; }
        _waiting = true;
        _time = _timeWait;
    }
    public void ReleaseNextItem()
    {
        (ItemHolder, int) k = _items.Dequeue();
        _moneyForShopping += _menuController.GetPrice(k.Item1.GetItemIndificator()) * k.Item2;
        for (int i = 0; i < k.Item2; i++)
        {
            k.Item1.DestroyLastItem();
        }
    }

    public void OnEndAction()
    {
        _onStartAction();
    }

    public void OnCashAction()
    {
        _priceMenuController.AddMoney(_moneyForShopping);
    }

    public void SetOnCreatedAction(Action start)
    {
        _onStartAction = start;
    }
}
