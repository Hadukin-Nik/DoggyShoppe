using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class BalanceModel
{
    private int balance;
    public int Balance 
    {
        get
        {
            return balance;
        }
        set
        {
            balance = value;
        } 
    }
    public BalanceModel()
    {
        Balance = 0;
    }

    public void AddMoney(int d)
    {
        Balance += d;
    }
    public void RemoveMoney(int d)
    {
        Balance -= d;
    }
    public void SetMoney(int d)
    {
        balance = d;
    }
}
