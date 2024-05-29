using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemsConsts;

public class ShopUIModel : BaseUIModel
{
    public ShopUIModel(GameObject prefab, Transform parent) : base(prefab, parent) { }

    public override void ButtonClick(PricePanel panel)
    {
        if (panel.Input != -1)
        {
            _priceMap[panel.Id] = panel.Input;
        }
        else
        {
            Debug.Log("Wrong input");
        }
    }
}
