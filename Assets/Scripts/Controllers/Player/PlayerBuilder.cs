using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static BuildingsConsts;
using static ItemsConsts;

public class PlayerBuilder : MonoBehaviour
{
    [SerializeField] private Transform _face;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private BuildingsFactory _buildingsFactory;
    [SerializeField] private float _raycastField;
    [SerializeField] private float _rotationSpeed;

    private Floor _floorController;
    private OrdersController _storageController; 
    private GameObject buildingGO = null;

    private Building _building;
    private bool _isBuilding;
    private float _wheel;
    private BuildingIndificator _buildingIndificator;

    private static event Action<String> onBuildingIdChange;
    private static event Action<bool> onBuildingRequest;
    public static event Action<bool> OnBuildingRequest
    {
        add
        {
            onBuildingRequest += value;
        }
        remove
        {
            onBuildingRequest -= value;
        }
    }
    public static event Action<String> OnBuildingIdChange
    {
        add
        {
            onBuildingIdChange += value;
        }
        remove
        {
            onBuildingIdChange -= value;
        }
    }


    private void Start()
    {
        _wheel = 0;
        _floorController = FindObjectOfType<Floor>();
        _storageController = FindObjectOfType<OrdersController>();
    }
    void Update()
    {
        RaycastHit hit;
        Vector3 fwd = _face.forward;


        Debug.DrawRay(_face.position + fwd * 1, fwd, Color.blue);
        //Move
        if(_isBuilding && Physics.Raycast(_face.position, fwd, out hit, _raycastField, _groundMask))
        {
            buildingGO.transform.position = new Vector3(hit.point.x, hit.point.y + _building._size.y/2, hit.point.z);
        }

        //Change
        _wheel = _wheel + Input.GetAxis("Mouse ScrollWheel");
        if (_wheel * _wheel > 0.001f)
        {
            Debug.Log("NOT ZERO: " + _wheel);
        }
        
        if (_isBuilding && _wheel < -1f)
        {
            _wheel = 0;
            Debug.Log("RIGHT BRACKET");

            Destroy(buildingGO);

            if((int)_buildingIndificator + 1 >= Enum.GetValues(typeof(BuildingIndificator)).Length)
            {
                _buildingIndificator = 0;
            } else
            {
                _buildingIndificator += 1;
            }

            _building = _buildingsFactory.Get(_buildingIndificator);

            while(_building == null)
            {
                if ((int)_buildingIndificator + 1 >= Enum.GetValues(typeof(BuildingIndificator)).Length)
                {
                    _buildingIndificator = 0;
                }
                else
                {
                    _buildingIndificator += 1;
                }
                _building = _buildingsFactory.Get(_buildingIndificator);
            }
            onBuildingIdChange?.Invoke(_building.getName());

            buildingGO = Instantiate(_building._gameBody);
        } else if (_isBuilding && _wheel > 1f)
        {
            _wheel = 0;
            Debug.Log("LEFT BRACKET");

            Destroy(buildingGO);

            if ((int)_buildingIndificator - 1 < 0) 
            {
                _buildingIndificator = (BuildingIndificator)(Enum.GetValues(typeof(BuildingIndificator)).Length - 1);
            }
            else
            {
                _buildingIndificator --;
            }

            _building = _buildingsFactory.Get(_buildingIndificator);

            while (_building == null)
            {
                if ((int)_buildingIndificator - 1 < 0)
                {
                    _buildingIndificator = (BuildingIndificator)(Enum.GetValues(typeof(BuildingIndificator)).Length - 1);
                }
                else
                {
                    _buildingIndificator--;
                }

                _building = _buildingsFactory.Get(_buildingIndificator);
            }
            onBuildingIdChange?.Invoke(_building.getName());

            buildingGO = Instantiate(_building._gameBody);
        }

        //rotating
        if (_isBuilding)
        {
           
            buildingGO.transform.Rotate(buildingGO.transform.up, _rotationSpeed * Time.deltaTime * Input.GetAxis("BuildingsRotate"));

        }
        BuildingContoller buildingContoller = null;
        if (_building != null && _isBuilding)
        {
            buildingContoller = buildingGO.GetComponent<BuildingContoller>();
            buildingContoller.SetData(_building);
        }

        //Open\Close\Create\Destroy
        if (Input.GetKeyDown(KeyCode.Z) && Physics.Raycast(_face.position, fwd, out hit, _raycastField) && (hit.transform.CompareTag("Building") || hit.transform.CompareTag("Storage"))) {

            BuildingContoller buildingContollerDelete = hit.transform.GetComponent<BuildingContoller>();
            _floorController.ReleaseBuildingPoints(buildingContollerDelete.getUsedPoints());
            buildingContollerDelete.SetUsedPoints(null);

            if (_building._gameBody.tag.Equals("Storage"))
            {
                _storageController.DeleteStorage(hit.transform.GetComponent<BoxStorage>());
            }
            Destroy(hit.transform.gameObject);
        }
        else if (_building == null && Input.GetKeyDown(KeyCode.B) && !_isBuilding && Physics.Raycast(_face.position, fwd, out hit, _raycastField, _groundMask)) {
            _floorController.CreateBuildingMap();
            _building = _buildingsFactory.Get(BuildingIndificator.Desk1);
            onBuildingIdChange?.Invoke(_building.getName());
            buildingGO = Instantiate(_building._gameBody, transform.position, transform.rotation);

            _buildingIndificator = BuildingIndificator.Desk1;
            _isBuilding = true;

            onBuildingRequest?.Invoke(true);
        }
        else if (_isBuilding && Input.GetKeyDown(KeyCode.Q)
            && _floorController.IsItPossibleToBuild(buildingContoller)){
            buildingGO.GetComponentInChildren<BoxCollider>().isTrigger = false;
            _floorController.TryToBuild(buildingContoller);
            _floorController.DestroyBuildingMap();

            if(_building._gameBody.tag.Equals("Storage"))
            {
                buildingGO.GetComponent<BoxStorage>().Init();
                _storageController.AddStorage(buildingGO.GetComponent<BoxStorage>());
            } else
            {
                List<ItemHolder> itemHolders = buildingGO.GetComponent<BuildingContoller>().GetItemHolders();

                foreach(ItemHolder itemHolder in itemHolders)
                {
                    itemHolder.Init();
                }
            }

            _building = null;
            _isBuilding = false;
            onBuildingRequest?.Invoke(false);

        }
        else if (_isBuilding && Input.GetKeyDown(KeyCode.Escape)) {
            _floorController.DestroyBuildingMap();

            _isBuilding = false;
            Destroy(buildingGO);

            _building = null;
        }
    }
}
