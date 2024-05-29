using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ItemsConsts;
using System.Linq;

public abstract class BaseUIModel
{
    private Dictionary<ItemIndificator, int> _priceMap;
    private List<PricePanel> _panelInstances;

    public Dictionary<ItemIndificator, int> PriceMap
    {
        get
        {
            return _priceMap;
        }
    }
    public List<PricePanel> PanelInstances
    {
        get
        {
            return _panelInstances;
        }
    }

    public abstract void ButtonClick(PricePanel p);

    public BaseUIModel(GameObject prefab, Transform parent)
    {
        _priceMap = new Dictionary<ItemIndificator, int>();
        _panelInstances = new List<PricePanel>();
        LoadPrices();
        LoadPanels(prefab, parent);
    }

    public void LoadPrices()
    {
        int startPrice = 10;
        foreach (ItemIndificator item in Enum.GetValues(typeof(ItemIndificator)))
        {
            if(item.Equals(ItemIndificator.Empty))
            {
                continue;
            }
            _priceMap.Add(item, startPrice);
            startPrice += 5;
            Debug.Log($"Loading price for {item.ToString()} : {startPrice}");
        }
    }

    public void LoadPanels(GameObject prefab, Transform parent)
    {
        foreach (var item in _priceMap)
        {
            GameObject p = UnityEngine.Object.Instantiate(prefab, parent);
            Debug.Log("Creating gameobject in parent");
            PricePanel panel = new(p)
            {
                Name = item.Key.ToString(),
                Price = item.Value
            };
            Debug.Log("Initialising panel");
            panel.Button.onClick.AddListener(() => ButtonClick(panel));
            panel.Button.onClick.AddListener(() => UpdatePrices());
            Debug.Log("Setting up buttons");
            _panelInstances.Add(panel);
            Debug.Log("Saving panel");
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
