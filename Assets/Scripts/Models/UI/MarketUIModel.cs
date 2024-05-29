using System;
using UnityEngine;
using static ItemsConsts;

public class MarketUIModel : BaseUIModel
{
    private event Action<ItemIndificator, int, int> _onItemOrderRequest;

    public event Action<ItemIndificator, int, int> OnItemOrderRequest
    {
        add
        {
            _onItemOrderRequest += value;
        }
        remove
        {
            _onItemOrderRequest -= value;
        }
    }

    public MarketUIModel(GameObject prefab, Transform parent) : base(prefab, parent)
    {

    }
    public override void ButtonClick(PricePanel p)
    {
        _onItemOrderRequest?.Invoke(p.Id, p.Price, p.Input);
    }
}
