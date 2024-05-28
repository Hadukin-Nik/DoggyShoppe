using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static ItemsConsts;

public abstract class BasePanelSetup : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI itemName;
    [SerializeField] protected TextMeshProUGUI itemPrice;
    [SerializeField] protected TMP_InputField itemPriceInput;
    [SerializeField] protected Button priceButton;

    public abstract void Setup(ItemPrice price, Action<ItemIndificator, int> onSpawnRequest, Action<ItemPrice> onChangePrice, Action onUpdatePriceMenu);

    protected void BaseSetup(ItemPrice price, Action<ItemIndificator, int> onSpawnRequest, Action<ItemPrice> onChangePrice, Action onUpdatePriceMenu)
    {
        itemName.text = price.item.ToString();
        itemPrice.text = price.price.ToString();
        priceButton.onClick.AddListener(() => onChangePrice(price));
        priceButton.onClick.AddListener(() => onUpdatePriceMenu());
    }
}
