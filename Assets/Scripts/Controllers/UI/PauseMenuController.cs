using System.Collections.Generic;
using UnityEngine;
using System;
using static MenuStates;
using static ItemsConsts;
using static BuildingsConsts;
using TMPro;
using UnityEngine.Audio;

public class PauseMenuController : MonoBehaviour
{
    public AudioMixer mixer;
    public EconomyController economyController;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI buildingText;
    private PauseMenuModel model;
    public Canvas pauseCanvas;
    public Canvas settingsCanvas;
    public Canvas priceCanvas;
    public Canvas ingameCanvas;
    public Canvas orderCanvas;
    public Canvas buildingCanvas;
    public BuildingIndificator buildingId = 0;

    private float scrollInput;

    ShopUIModel shopModel;
    MarketUIModel marketModel;

    public GameObject shopPanelPrefab;
    public Transform shopPanelParent;
    public GameObject marketPanelPrefab;
    public Transform marketPanelParent;

    public Dictionary<ItemIndificator, int> marketPriceMap = new Dictionary<ItemIndificator, int>();
    private List<GameObject> marketPanelInstances = new List<GameObject>();
    private float marketTimer = 0f;
    private float marketInterval = 5f;
    //temp

    private Action<ItemIndificator, int> _onSpawnBoxRequest;

    private Action<String> _onBuildingIdChange;

    private void Start()
    {
        LoadModels();
        SetupUI();
        //LoadShopPrices();
        //LoadMarketPrices();
        _onSpawnBoxRequest += TestMethod;
        marketModel.OnItemOrderRequest += ItemOrderRequestCheck;
        PlayerBuilder.OnBuildingRequest += ToggleBuildingMenu;
        PlayerBuilder.OnBuildingIdChange += UpdateBuildingText;
    }

    public void ItemOrderRequestCheck(ItemIndificator item, int price, int count)
    {
        Debug.Log($"Invoke order request {item} : {price} : {count}");
        if (_onSpawnBoxRequest != null)
        {

            if (count > 0 && economyController.HaveEnough(count * price))
            {
                _onSpawnBoxRequest(item, count);
            }
        }
        else
        {
            Debug.Log("No subscribers for OnSpawnBoxRequest event");
        }
    }

    public void LoadModels()
    {
        model = new();
        shopModel = new(shopPanelPrefab, shopPanelParent);
        marketModel = new(marketPanelPrefab, marketPanelParent);
    }
    public void SetupUI()
    {
        model.Close(pauseCanvas);
        model.Close(settingsCanvas);
        model.Close(priceCanvas);
        model.Close(orderCanvas);
        model.Close(buildingCanvas);
    }

    void TestMethod(ItemIndificator name, int c)
    {
        Debug.Log($"Invoke box spawn {name} : {c}");
    }
    void UpdateBuildingText(String name)
    {
        buildingText.text = name;
    }
    void ToggleBuildingMenu(bool b)
    {
        if (model.state == MenuState.None)
        {
            OpenMenu(buildingCanvas);
            ChangeState(MenuState.BuildingMenu);
        }
        else if (model.state == MenuState.BuildingMenu)
        {
            CloseMenu(buildingCanvas);
            ChangeState(MenuState.None);
        }
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
                case MenuState.BuildingMenu:
                    {
                        CloseMenu(buildingCanvas);
                        ChangeState(MenuState.None);
                        break;
                    }
            }
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(model.state == MenuState.None)
            {
                PauseGame();
                shopModel.UpdatePrices();
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
        /*if(Input.GetKeyDown(KeyCode.Q))
        {
            if(model.state == MenuState.None)
            {
                OpenMenu(buildingCanvas);
                ChangeState(MenuState.BuildingMenu);
            }
            else if(model.state == MenuState.BuildingMenu)
            {
                CloseMenu(buildingCanvas);
                ChangeState(MenuState.None);
            }
        }*/
        if(Input.GetKeyDown(KeyCode.T))
        {
            if (model.state == MenuState.None)
            {
                PauseGame();
                marketModel.UpdatePrices();
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

    public void SetVolume(float v)
    {
        mixer.SetFloat("Volume", v);
    }

    public int GetGlobalMarketPrice(ItemIndificator item)
    {
        return marketModel.PriceMap[item];
    }

    public int GetCurrentPrice(ItemIndificator item)
    {
        return shopModel.PriceMap[item];
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

        foreach (var item in marketModel.PanelInstances)
        {
            if (item.Price >= 10)
            {
                updates[item.Id] = item.Price + UnityEngine.Random.Range(-5, 5);
            }
            else
            {
                updates[item.Id] = item.Price + UnityEngine.Random.Range(0, 5);
            }
        }

        foreach (var update in updates)
        {
            marketModel.PriceMap[update.Key] = update.Value;
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

    public void AddActionOnOrderEvent(Action<ItemIndificator, int> action)
    {
        _onSpawnBoxRequest += action;
    }

    public void DeleteMoneyAction(ItemIndificator itemIndificator, int count)
    {
        economyController.RemoveMoney(count * GetGlobalMarketPrice(itemIndificator));
    }
}