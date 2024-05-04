using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EconomyController : MonoBehaviour
{
    private static EconomyController instance;
    private EconomyModel model;
    public TextMeshProUGUI balanceText;

    public static EconomyController Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        //singleton ne obyazatelen
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        model = new EconomyModel(0);
        UpdateBalanceDisplay();
    }
    public void AddMoney(int amount = 100)
    {
        model.AddMoney(amount);
        UpdateBalanceDisplay();
    }

    public void RemoveMoney(int amount = 100)
    {
        model.RemoveMoney(amount);
        UpdateBalanceDisplay();
    }

    public void ResetBalance()
    {
        model.ResetBalance();
        UpdateBalanceDisplay();
    }

    private void UpdateBalanceDisplay()
    {
        balanceText.text = model.Balance.ToString();
    }
}
