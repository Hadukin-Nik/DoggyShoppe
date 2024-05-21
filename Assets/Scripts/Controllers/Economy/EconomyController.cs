using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class EconomyController : MonoBehaviour
{
    private static EconomyController instance;
    private EconomyModel model;
    [SerializeField]
    private int balance;
    public TextMeshProUGUI balanceText;

    public static EconomyController Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        //singleton not required
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
        balance = model.Balance;
        UpdateBalanceDisplay();
    }
    private void Update()
    {
        model.Balance = balance;
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

    public void SetBalance(int newB)
    {
        balance = newB;
    }

    private void UpdateBalanceDisplay()
    {
        balanceText.text = model.Balance.ToString();
    }
}
