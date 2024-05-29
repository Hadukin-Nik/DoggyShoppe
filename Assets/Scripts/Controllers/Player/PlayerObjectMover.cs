using UnityEngine;


public class PlayerObjectMover : MonoBehaviour
{
    [SerializeField] private Transform _itemPlaceHolder;
    [SerializeField] private Transform _face;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Vector3 _standartBoxSize = new Vector3(1, 0, 1);


    [SerializeField] private float _raycastField;
    [SerializeField] private float _raycastDistanceOffset;

    private ItemBox _itemInHands;
    private Transform _itemInHandsBody;
    private Floor _floor;

    private void Start()
    {
        _floor = FindAnyObjectByType<Floor>();
    }

    private void Update()
    {
        if(_itemInHandsBody != null)
        {
            _itemInHandsBody.position = _itemPlaceHolder.transform.position;
            _itemInHandsBody.rotation = _itemPlaceHolder.transform.rotation;    
        }
        RaycastHit hit;
        Vector3 fwd = _face.forward;
        Debug.DrawRay(_face.position, fwd, Color.yellow);
        Debug.DrawRay(_face.position + fwd.normalized * _raycastDistanceOffset, fwd, Color.red);

        if (_itemInHandsBody == null && Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_face.position, fwd, out hit,_raycastField) && hit.transform.CompareTag("Item"))
        {
            _itemInHands = hit.transform.GetComponent<ItemBox>();
            _itemInHandsBody = hit.transform;

            hit.transform.GetComponent<BoxCollider>().enabled = false;

            if(_itemInHands.GetUsedPoints() != null)
            {
                _floor.ReleaseBuildingPoints(_itemInHands.GetUsedPoints());
                _itemInHands.SetUsedPoints(null);
            } else if (_itemInHands.getBoxStorage() != null)
            {
                _itemInHands.getBoxStorage().Displace(_itemInHands.WherePlaced());
                _itemInHands.SetStorage(null);
            }
        }  else if (_itemInHandsBody != null && Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_face.position, fwd, out hit, _raycastField) && hit.transform.CompareTag("ItemHolder"))
        {
            ItemHolder board = hit.transform.GetComponent<ItemHolder>();

            if(board.IsItemPlaceable(_itemInHands.GetItemIndificator(), Mathf.Min(_itemInHands.CountOfItems(), board.FreeSpace())) && _itemInHands.CountOfItems() > 0)
            {
                int toAdd = Mathf.Min(_itemInHands.CountOfItems(), board.FreeSpace());
                board.AddNewItem(_itemInHands.GetItemIndificator(), toAdd);
                _itemInHands.SetCountOfItems(_itemInHands.CountOfItems() - toAdd);

                if(_itemInHands.CountOfItems() <= 0)
                {
                    Destroy(_itemInHands.gameObject);
                    _itemInHands = null;
                }
            }
        } else if (_itemInHandsBody != null && Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_face.position, fwd, out hit, _raycastField) && hit.transform.CompareTag("Storage"))
        {
            BoxStorage storage = hit.transform.GetComponent<BoxStorage>();

            if(storage.IsPlaceable())
            {
                (Vector3, Quaternion, (int, int)) newTransform = storage.Place();

                _itemInHands.SetStorage(storage);
                _itemInHands.SetWherePlaced(newTransform.Item3);
                _itemInHands.transform.position = newTransform.Item1;
                _itemInHands.transform.rotation = newTransform.Item2;
                _itemInHandsBody.transform.GetComponent<BoxCollider>().enabled = true;

                _itemInHands = null;
                _itemInHandsBody = null;
            }
        }
        else if (_itemInHandsBody != null && Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_face.position, fwd, out hit, _raycastField, _groundMask)
            && _floor.IsItPossibleToBuild((new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z), transform.rotation), _standartBoxSize))
        {
            _itemInHandsBody.position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);

            _itemInHandsBody.transform.GetComponent<BoxCollider>().enabled = true;

            _itemInHands.transform.rotation = transform.rotation;

            _itemInHands.SetUsedPoints(_floor.TryToBuild((_itemInHandsBody.position, transform.rotation), _standartBoxSize));

            _itemInHands = null;
            _itemInHandsBody = null;
        }
    }
}
