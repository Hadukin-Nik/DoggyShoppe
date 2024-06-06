using System;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{
    [SerializeField] private float _height = 1f;
    [SerializeField] private int _minMoneyAmount = 0;
    [SerializeField] private int _maxMoneyAmount = 100;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(_time <= 0f && _waiting)
        {
            try
            {
                bool isCreated = tryCreatePath();

                if(!isCreated)
                {
                    OnEndAction();
                }
            }
            catch (Exception e)
            {
                OnEndAction();
            }

        }
        _time -= Time.deltaTime;
    }

    private bool tryCreatePath()
    {
        _waiting = false;
        Vector3 point = transform.position;

        List<Vector3> points = new List<Vector3>();
        Queue<int> itemHolders = new Queue<int>();
        int cashIndex = 0;
        List<(ItemHolder, int)> holdersItems = new List<(ItemHolder, int)>();
        HashSet<ItemHolder> visited = new HashSet<ItemHolder>();
        int lengthWay = UnityEngine.Random.Range(1, 4);
        _items = new Queue<(ItemHolder, int)>();

        int money = UnityEngine.Random.Range(_minMoneyAmount, _maxMoneyAmount);

        for (int l = 0; l < lengthWay; l++)
        {

            (List<Vector3>, ItemHolder) points2 = _floorController.GetWayToRandom(point);
            ItemHolder item = points2.Item2;

            if (item == null || item.getFreeItems() == 0 || visited.Contains(item) || money < _menuController.GetGlobalMarketPrice(item.GetItemIndificator()))
            {
                continue;
            }
            int use = UnityEngine.Random.Range(1, Math.Max(1, Math.Min(money / _menuController.GetCurrentPrice(item.GetItemIndificator()), item.getFreeItems())));

            money -= use * _menuController.GetGlobalMarketPrice(item.GetItemIndificator());

            visited.Add(item);
            points.AddRange(points2.Item1);
            point = points[points.Count - 1];
            itemHolders.Enqueue(points.Count - 1);
            
            _items.Enqueue((item, use));
            holdersItems.Add((item, item.getFreeItems() - use));
        }

        if (_items.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < 5; i++)
        {
            List<Vector3> points2 = _floorController.GetWayToRandomEndPoint(point);

            if (points2 != null)
            {
                points.AddRange(points2);
                point = points[points.Count - 1];
                cashIndex = points.Count - 1;
                break;
            }

            if (points2 == null && i == 4)
            {
                return false;
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
                return false;
            }
        }
        foreach (var i in holdersItems)
        {
            i.Item1.setFreeItems(i.Item2);
        }
        _mover.SetHeight(_height);
        _mover.SetActionOnEach(ReleaseNextItem);
        _mover.SetActionOnEnd(OnCashAction);

        _mover.Move(points, itemHolders, cashIndex);
        _mover.DestroyAction(OnEndAction);
        _onStartAction();

        return true;

    }

    public void ReleaseNextItem()
    {
        (ItemHolder, int) k = _items.Dequeue();
        _moneyForShopping += _menuController.GetGlobalMarketPrice(k.Item1.GetItemIndificator()) * k.Item2;
        for (int i = 0; i < k.Item2; i++)
        {
            k.Item1.DestroyLastItem();
        }
    }

    public void OnEndAction()
    {
        Destroy(gameObject);
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
