using System.Collections.Generic;
using UnityEngine;
using System;
using static MenuStates;
using static ItemsConsts;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class PauseMenuController : MonoBehaviour
{
    public AudioMixer mixer;
    public EconomyController economyController;
    public TextMeshProUGUI balanceText;
    private PauseMenuModel model;
    public Canvas pauseCanvas;
    public Canvas settingsCanvas;
    public Canvas priceCanvas;
    public Canvas ingameCanvas;
    public Canvas orderCanvas;

    public GameObject shopPanelPrefab;
    public Transform shopPanelParent;
    public GameObject marketPanelPrefab;
    public Transform marketPanelParent;
    public Dictionary<ItemIndificator, int> shopPriceMap = new Dictionary<ItemIndificator, int>();
    public Dictionary<ItemIndificator, int> marketPriceMap = new Dictionary<ItemIndificator, int>();
    private List<GameObject> shopPanelInstances = new List<GameObject>();
    private List<GameObject> marketPanelInstances = new List<GameObject>();
    private float marketTimer = 0f;
    private float marketInterval = 5f;
    //temp
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemPrice;
    private TMP_InputField itemInput;
    private Button priceButton;

    public static event Action<ItemIndificator, int> OnSpawnBoxRequest;

    private void Start()
    {
        model = new PauseMenuModel();
        model.Close(pauseCanvas);
        model.Close(settingsCanvas);
        model.Close(priceCanvas);
        model.Close(orderCanvas);
        LoadShopPrices();
        LoadMarketPrices();
        OnSpawnBoxRequest += TestMethod;
    }
    void TestMethod(ItemIndificator name, int c)
    {
        Debug.Log($"Invoke event {name} : {c}");
    }

    private void Update()
    {
        MarketTimerTick();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            switch(model.state)
            {
                case MenuState.None:
                    {
                        PauseGame();
                        OpenMenu(pauseCanvas);
                        ChangeState(MenuState.PauseMenu);
                        break;
                    }
                case MenuState.PauseMenu:
                    {
                        CloseMenu(pauseCanvas);
                        ResumeGame();
                        ChangeState(MenuState.None);
                        break;
                    }
                case MenuState.SettingsMenu:
                    {
                        CloseMenu(settingsCanvas);
                        OpenMenu(pauseCanvas);
                        ChangeState(MenuState.PauseMenu);
                        break;
                    }
                case MenuState.PriceMenu:
                    {
                        CloseMenu(priceCanvas);
                        ResumeGame();
                        ChangeState(MenuState.None);
                        break;
                    }
            }
            if(model.state == MenuState.PriceMenu)
            {

            }
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            if(model.state == MenuState.None)
            {
                PauseGame();
                UpdatePriceMenu();
                OpenMenu(priceCanvas);
                ChangeState(MenuState.PriceMenu);
            }
            else if(model.state == MenuState.PriceMenu)
            {
                CloseMenu(priceCanvas);
                ResumeGame();
                ChangeState(MenuState.None);
            }
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            if (model.state == MenuState.None)
            {
                PauseGame();
                UpdatePriceMenu();
                OpenMenu(orderCanvas);
                ChangeState(MenuState.OrderMenu);
            }
            else if (model.state == MenuState.OrderMenu)
            {
                CloseMenu(orderCanvas);
                ResumeGame();
                ChangeState(MenuState.None);
            }
        }
    }
    #region UI
    public void PauseGame()
    {
        model.Pause();
    }

    public void ResumeGame()
    {
        model.Resume();
    }
    public void OpenMenu(Canvas c)
    {
        model.Open(c);
    }
    public void CloseMenu(Canvas c)
    {
        model.Close(c);
    }
    public void LoadScene(string s)
    {
        model.LoadScene(s);
    }
    public void ChangeState(MenuState state)
    {
        model.state = state;
    }
    public void ChangeState(int i)
    {
        if(Enum.IsDefined(typeof(MenuState), i))
        {
            model.state = (MenuState)i;
        }
        else
        {
            UnityEngine.Debug.Log("???????????? ????? ?????????");
        }
    }
    #endregion
    //test
    public void UpdatePriceMenu()
    {
        foreach (var item in shopPriceMap)
        {
            shopPanelInstances[(int)item.Key].transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = item.Value.ToString();
        }
    }
    public void ChangePrice()
    {
        /*ItemPrice ip = prices.First(ip => ip.item == (ItemsConsts.ItemIndificator)Enum.Parse(typeof(ItemsConsts.ItemIndificator), name, true));
        UnityEngine.Debug.Log($"{name} : {price}");
        ip.price = Int32.Parse(price);*/
        Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        Transform pricePanel = button.transform.parent;
        itemName = pricePanel.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemInput = pricePanel.transform.Find("ItemPriceInput").GetComponent<TMP_InputField>();
        int price = GetInput(itemInput);
        if (price != -1)
        {
            shopPriceMap[(ItemIndificator)Enum.Parse(typeof(ItemIndificator), itemName.text, true)] = GetInput(itemInput);
        }
    }

    public void UpdateGameUI()
    {

    }
    public void SetVolume(float v)
    {
        mixer.SetFloat("Volume", v);
    }

    public int GetPrice(ItemIndificator item)
    {
        return marketPriceMap[item];
    }

    void TriggerSpawnBoxEvent()
    {
        if (OnSpawnBoxRequest != null)
        {
            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            Transform pricePanel = button.transform.parent;
            itemName = pricePanel.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            itemInput = pricePanel.transform.Find("ItemPriceInput").GetComponent<TMP_InputField>();
            ItemIndificator item = (ItemIndificator)Enum.Parse(typeof(ItemIndificator), itemName.text, true);
            int count = GetInput(itemInput);
            if(count > 0)
            {
                OnSpawnBoxRequest(item, count);
            }
        }
        else
        {
            Debug.Log("No subscribers for OnSpawnBoxRequest event");
        }
    }
    void LoadShopPrices()
    {
        int startPrice = 10;
        foreach (ItemIndificator item in Enum.GetValues(typeof(ItemIndificator)))
        {
            shopPriceMap.Add(item, startPrice);
            startPrice += 5;
        }
        foreach (var item in shopPriceMap)
        {
            GameObject pricePanel = Instantiate(shopPanelPrefab, shopPanelParent.transform);
            Debug.Log("Created panel");
            RectTransform panelRectTransform = pricePanel.GetComponent<RectTransform>();
            panelRectTransform.localScale = Vector3.one;

            shopPanelInstances.Add(pricePanel);

            itemName = pricePanel.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            itemPrice = pricePanel.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>();
            itemInput = pricePanel.transform.Find("ItemPriceInput").GetComponent<TMP_InputField>();
            priceButton = pricePanel.transform.Find("ChangePriceBtn").GetComponent<Button>();
            priceButton.onClick.AddListener(() => ChangePrice());
            priceButton.onClick.AddListener(() => UpdatePriceMenu());
            itemName.text = item.Key.ToString();
            itemPrice.text = item.Value.ToString();
        }
    }
    void LoadMarketPrices()
    {
        int startPrice = 0;
        foreach (ItemIndificator item in Enum.GetValues(typeof(ItemIndificator)))
        {
            marketPriceMap.Add(item, startPrice);
            startPrice += 5;
        }
        foreach (var item in marketPriceMap)
        {
            GameObject pricePanel = Instantiate(marketPanelPrefab, marketPanelParent.transform);
            RectTransform panelRectTransform = pricePanel.GetComponent<RectTransform>();
            panelRectTransform.localScale = Vector3.one;

            marketPanelInstances.Add(pricePanel);

            itemName = pricePanel.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            itemPrice = pricePanel.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>();
            itemInput = pricePanel.transform.Find("ItemPriceInput").GetComponent<TMP_InputField>();
            priceButton = pricePanel.transform.Find("ChangePriceBtn").GetComponent<Button>();
            priceButton.onClick.AddListener(() => TriggerSpawnBoxEvent());
            itemName.text = item.Key.ToString();
            itemPrice.text = item.Value.ToString();
        }
    }
    int GetInput(TMP_InputField inputField)
    {
        int r;
        try
        {
            r = Int32.Parse(inputField.text);
            inputField.image.color = Color.white;
        }
        catch
        {
            inputField.image.color = Color.red;
            return -1;
        }
        return r >= 0 ? r : -1;
    }

    public void UpdateMarketMenu()
    {
        foreach(var item in marketPriceMap)
        {
            marketPanelInstances[(int)item.Key].transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = item.Value.ToString();
        }
    }
    public void RandomizeMarketPrices()
    {
        Dictionary<ItemIndificator, int> updates = new Dictionary<ItemIndificator, int>();

        foreach (var item in marketPriceMap)
        {
            if (item.Value >= 10)
            {
                updates[item.Key] = item.Value + UnityEngine.Random.Range(-5, 5);
            }
            else
            {
                updates[item.Key] = item.Value + UnityEngine.Random.Range(0, 5);
            }
        }

        foreach (var update in updates)
        {
            marketPriceMap[update.Key] = update.Value;
        }
        Debug.Log("Randomize market prices");
    }
    public void MarketTimerTick()
    {
        marketTimer += Time.deltaTime;

        if (marketTimer >= marketInterval)
        {
            RandomizeMarketPrices();
            UpdateMarketMenu();
            marketTimer = 0f;
        }
    }
}