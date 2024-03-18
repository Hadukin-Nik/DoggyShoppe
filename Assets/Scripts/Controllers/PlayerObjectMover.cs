using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

public class PlayerObjectMover : MonoBehaviour
{
    [SerializeField] private Transform _itemPlaceHolder;
    [SerializeField] private Transform _face;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private float _raycastField;
    [SerializeField] private float _raycastDistanceOffset;

    private ItemBox _itemInHands;
    private Transform _itemInHandsBody;
    private int _hitCounter = 0;
    private RaycastHit _lastHit;
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
        } else if (_itemInHandsBody != null && Input.GetKeyDown(KeyCode.E) && Physics.Raycast(_face.position + fwd.normalized * _raycastDistanceOffset, fwd, out hit, _raycastField) && (_groundMask & (1 << hit.transform.gameObject.layer)) != 0)
        {
            _itemInHandsBody.position = hit.point;
            _lastHit = hit;
            _itemInHands = null;
            _itemInHandsBody = null;
        }
    }
}
