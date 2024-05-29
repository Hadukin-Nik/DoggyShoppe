using UnityEngine;

public class ShopUIModel : BaseUIModel
{
    public ShopUIModel(GameObject prefab, Transform parent) : base(prefab, parent) { }

    public override void ButtonClick(PricePanel panel)
    {
        if (panel.Input != -1)
        {
            PriceMap[panel.Id] = panel.Input;
        }
        else
        {
            Debug.Log("Wrong input");
        }
    }
}
