using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;
using UnityEditor;

[Serializable]
public class ItemPriceController : MonoBehaviour
{
    public Canvas menuCanvas;
    public GameObject panelPrefab;
    public Transform panelParent;
    public List<ItemPrice> prices = new List<ItemPrice>();
    private PauseMenuModel model;
    private TextMeshProUGUI itemName;
    private TextMeshProUGUI itemPrice;
    private TMP_InputField itemPriceInput;
    private Button priceButton;
    private List<GameObject> panelInstances = new List<GameObject>();

    void Start()
    {
        menuCanvas.gameObject.SetActive(false);
        model = new PauseMenuModel();
        // ?????????? ??????
        foreach (ItemsConsts.ItemIndificator item in Enum.GetValues(typeof(ItemsConsts.ItemIndificator)))
        {
            prices.Add(new ItemPrice(item, 0));
        }
        for(int i = 0; i < prices.Count; i++)
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
            itemName.text = prices[i].item.ToString();
            itemPrice.text = prices[i].price.ToString();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (model.isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
                UpdateUI();
            }
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        model.TogglePause();
        menuCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        model.TogglePause();
        menuCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < prices.Count; i++)
        {
            panelInstances[i].transform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().text = prices[i].price.ToString();
        }
    }
    public void ChangePrice()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        Button button = clickedButton.GetComponent<Button>();
        Transform pricePanel = button.transform.parent; 
        itemName = pricePanel.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        itemPriceInput = pricePanel.transform.Find("ItemPriceInput").GetComponent<TMP_InputField>();
        ItemPrice foundItem = prices.Find(price => price.item == (ItemsConsts.ItemIndificator)Enum.Parse(typeof(ItemsConsts.ItemIndificator), itemName.text));
        foundItem.price = Int32.Parse(itemPriceInput.text);
    }
}
