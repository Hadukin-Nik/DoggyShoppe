using System.Collections.Generic;
using UnityEngine;
using System;
using static MenuStates;
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

    public GameObject panelPrefab;
    public Transform panelParent;
    public List<ItemPrice> prices = new List<ItemPrice>();
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemPrice;
    private TMP_InputField itemPriceInput;
    private Button priceButton;
    private List<GameObject> panelInstances = new List<GameObject>();

    private void Start()
    {
        model = new PauseMenuModel();
        model.Close(pauseCanvas);
        model.Close(settingsCanvas);
        model.Close(priceCanvas);

        // TODO refactoring
        int startPrice = 15;
        foreach (ItemsConsts.ItemIndificator item in Enum.GetValues(typeof(ItemsConsts.ItemIndificator)))
        {
            prices.Add(new ItemPrice(item, startPrice));
            startPrice += 5;
        }
        for (int i = 0; i < prices.Count; i++)
        {   
            GameObject pricePanel = Instantiate(panelPrefab, panelParent.transform);
            RectTransform panelRectTransform = pricePanel.GetComponent<RectTransform>();
            panelRectTransform.localScale = Vector3.one;
            panelInstances.Add(pricePanel);
            itemName = pricePanel.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            itemPrice = pricePanel.transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>();
            itemPriceInput = pricePanel.transform.Find("ItemPriceInput").GetComponent<TMP_InputField>();
            priceButton = pricePanel.transform.Find("ChangePriceBtn").GetComponent<Button>();
            priceButton.onClick.AddListener(() => ChangePrice());
            priceButton.onClick.AddListener(() => UpdatePriceMenu());
            itemName.text = prices[i].item.ToString();
            itemPrice.text = prices[i].price.ToString();
        }
    }

    private void Update()
    {
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
    }

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
    //test
    public void UpdatePriceMenu()
    {
        for (int i = 0; i < prices.Count; i++)
        {
            panelInstances[i].transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = prices[i].price.ToString();
        }
    }
    public void ChangePrice()
    {
        /*ItemPrice ip = prices.First(ip => ip.item == (ItemsConsts.ItemIndificator)Enum.Parse(typeof(ItemsConsts.ItemIndificator), name, true));
        UnityEngine.Debug.Log($"{name} : {price}");
        ip.price = Int32.Parse(price);*/
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        Button button = clickedButton.GetComponent<Button>();
        Transform pricePanel = button.transform.parent;
        itemName = pricePanel.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemPriceInput = pricePanel.transform.Find("ItemPriceInput").GetComponent<TMP_InputField>();
        ItemPrice foundItem = prices.Find(price => price.item == (ItemsConsts.ItemIndificator)Enum.Parse(typeof(ItemsConsts.ItemIndificator), itemName.text));
        foundItem.price = Int32.Parse(itemPriceInput.text);
        
    }

    public void UpdateGameUI()
    {

    }
    public void SetVolume(float v)
    {
        mixer.SetFloat("Volume", v);
    }

    public int GetPrice(ItemsConsts.ItemIndificator item)
    {
        foreach (ItemPrice itemPrice in prices)
        {
            if (itemPrice.item == item)
            {
                return itemPrice.price;
            }
        }

        Console.Error.WriteLine("Wrong item indifactor in pause menu controller");
        return 0;
    }
}
