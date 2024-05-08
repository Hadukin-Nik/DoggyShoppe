using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyModel
{
    private int balance;

    public int Balance
    {
        get { return balance; }
        set { balance = value; }
    }

    public EconomyModel(int startingBalance)
    {
        balance = startingBalance;
    }

    public void AddMoney(int amount)
    {
        balance += amount;
    }

    public void RemoveMoney(int amount)
    {
        balance -= amount;
    }

    public void ResetBalance()
    {
        balance = 0;
    }
}
