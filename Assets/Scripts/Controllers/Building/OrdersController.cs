using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrdersController : MonoBehaviour
{
    [SerializeField] GameObject _boxPrefab;

    private HashSetNListStructure<BoxStorage> _boxStorages;
    private PauseMenuController _menuController;
    void Start()
    {
        _menuController = FindAnyObjectByType<PauseMenuController>();
        _boxStorages = new HashSetNListStructure<BoxStorage>();
        _menuController.AddActionOnOrderEvent(TryAddNewOrder);
    }

    void Update()
    {
        
    }

    public void TryAddNewOrder(ItemsConsts.ItemIndificator itemIndificator, int count)
    {
        Debug.Log("NEW ORDER:" + itemIndificator.ToString() + " " + count);


        List<BoxStorage> boxStorages = _boxStorages.GetAll();
        
        for(int i = 0; i < boxStorages.Count; i++)
        {
            if (boxStorages[i].IsPlaceable())
            {
                (Vector3, Quaternion, (int, int)) newObject = boxStorages[i].Place();

                GameObject gameObject1 = Instantiate(_boxPrefab, newObject.Item1, newObject.Item2);
                ItemBox itemBox = gameObject1.GetComponent<ItemBox>();

                gameObject1.transform.GetComponent<BoxCollider>().enabled = true;

                itemBox.SetStorage(boxStorages[i]);
                itemBox.SetWherePlaced(newObject.Item3);
                itemBox.transform.position = newObject.Item1;
                itemBox.transform.rotation = newObject.Item2;
                itemBox.SetCountOfItems(count);
                itemBox.SetItem(itemIndificator);

                _menuController.DeleteMoneyAction(itemIndificator, count);
            }
        }
    }

    public void AddStorage(BoxStorage boxStorage)
    {
        _boxStorages.Add(boxStorage);
    }

    public void DeleteStorage(BoxStorage boxStorage)
    {
        _boxStorages.Remove(boxStorage);
    }
}
