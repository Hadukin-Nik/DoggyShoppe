using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;


public class PlayerObjectMover : MonoBehaviour
{
    [SerializeField] private Transform _itemPlaceHolder;
    [SerializeField] private Transform _face;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private float _raycastField;
    [SerializeField] private float _raycastDistanceOffset;

    private ItemBox _itemInHands;
    private Transform _itemInHandsBody;
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
        }  else if (_itemInHandsBody != null && Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_face.position, fwd, out hit, _raycastField) && hit.transform.CompareTag("ItemHolder"))
        {
            ItemHolder board = hit.transform.GetComponent<ItemHolder>();

            if(board.IsItemPlaceable(_itemInHands.GetItemIndificator()) && _itemInHands.tryDecreaseAmount())
            {
                board.AddNewItem(_itemInHands.GetItemIndificator());
            }
        }
        else if (_itemInHandsBody != null && Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_face.position, fwd, out hit, _raycastField, _groundMask))
        {
            _itemInHandsBody.position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);

            _itemInHandsBody.transform.GetComponent<BoxCollider>().enabled = true;

            _itemInHands.transform.rotation = transform.rotation;
            _itemInHands = null;
            _itemInHandsBody = null;
        }
    }

    
}
