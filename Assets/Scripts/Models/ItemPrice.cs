using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[Serializable]
public class ItemPrice
{
    public ItemsConsts.ItemIndificator item;
    public int price;

    public ItemPrice(ItemsConsts.ItemIndificator item, int price)
    {
        this.item = item;
        this.price = price;
    }

    public void ChangePrice(int i)
    {
        price = i;
    }
}
