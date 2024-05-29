using TMPro;
using UnityEngine;

[SerializeField]
public class EconomyController : MonoBehaviour
{
    public BalanceModel model;
    public Canvas IngameCanvas;
    public TextMeshProUGUI balanceText;
    [SerializeField]
    int balance;
    // Start is called before the first frame update
    private void Awake()
    {
        model = new BalanceModel();

    }
    void Start()
    {
        model.SetMoney(balance);
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
        model.SetMoney(balance);
    }

    public void AddMoney(int d)
    {
        balance += d;
        model.AddMoney(d);
        UpdateUI();
    }
    public void RemoveMoney(int d)
    {
        balance -= d;
        model.RemoveMoney(d);
        UpdateUI();
    }
    public void SetMoney(int d)
    {
        balance = d;
        model.SetMoney(d);
        UpdateUI();
    }

    public bool HaveEnough(int money)
    {
        return balance >= money;
    }
    public void UpdateUI()
    {
        balanceText.text = model.Balance.ToString();
    }
}
