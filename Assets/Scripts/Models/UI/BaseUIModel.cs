using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ItemsConsts;
using System.Linq;

public abstract class BaseUIModel
{
    protected readonly Dictionary<ItemIndificator, int> _priceMap;
    protected readonly List<PricePanel> _panelInstances;

    public abstract void ButtonClick(PricePanel p);

    public BaseUIModel(GameObject prefab, Transform parent)
    {

    }

    public void LoadPrices()
    {
        int startPrice = 10;
        foreach (ItemIndificator item in Enum.GetValues(typeof(ItemIndificator)))
        {
            if(item == ItemIndificator.Empty)
            {
                continue;
            }
            _priceMap.Add(item, startPrice);
            startPrice += 5;
        }
    }

    public void LoadPanels(GameObject prefab, Transform parent)
    {
        foreach (var item in _priceMap)
        {
            GameObject p = UnityEngine.Object.Instantiate(prefab, parent);
            PricePanel panel = new(p)
            {
                Name = item.Key.ToString(),
                Price = item.Value
            };
            panel.Button.onClick.AddListener(() => ButtonClick(panel));
            panel.Button.onClick.AddListener(() => UpdatePrices());
            _panelInstances.Add(panel);
        }
    }

    public void UpdatePrices()
    {
        foreach(var item in _priceMap)
        {
            PricePanel p = _panelInstances.First(panel => panel.Id == item.Key);
            p.Price = item.Value;
        }
    }
}
