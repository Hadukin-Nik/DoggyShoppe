using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPrice
{
    public ItemsConsts.ItemIndificator item;
    public int price;
    public ItemPrice(ItemsConsts.ItemIndificator item, int price)
    {
        this.item = item;
        this.price = price;
    }
}
