using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ItemsConsts;

public class PricePanel
{
    private readonly GameObject _panel;
    private readonly TextMeshProUGUI _name;
    private readonly TextMeshProUGUI _price;
    private readonly TMP_InputField _input;
    private readonly Button _button;

    public GameObject Panel
    {
        get { return _panel; }
    }
    public string Name
    {
        get
        {
            return _name.text;
        }
        set
        {
            _name.text = value;
        }
    }
    public int Price
    {
        get
        {
            try
            {
                return Int32.Parse(_price.text);
            }
            catch
            {
                //handle error
                return 0;
            }
        }
        set
        {
            _price.text = value.ToString();
        }
    }
    public int Input
    {
        get
        {
            if(CheckInput())
            {
                return Int32.Parse(_input.text);
            }
            else
            {
                return -1;
            }
        }
    }
    // check if input is correct
    public bool CheckInput()
    {
        int r;
        try
        {
            r = Int32.Parse(_input.text);
        }
        catch
        {
            // wrong input
            return false;
        }
        return r >= 0 && r <= 1000000000;
    }
    public Button Button
    {
        get
        {
            return _button;
        }
    }

    public ItemIndificator Id
    {
        get
        {
            return (ItemIndificator)Enum.Parse(typeof(ItemIndificator), _name.text, true);
        }
    }
    public PricePanel(GameObject panel)
    {
        Debug.Log("Initialisation");
        _panel = panel;
        _name = panel.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        _price = panel.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>();
        _input = panel.transform.Find("ItemPriceInput").GetComponent<TMP_InputField>();
        _button = panel.transform.Find("ChangePriceBtn").GetComponent<Button>();
    }
}
