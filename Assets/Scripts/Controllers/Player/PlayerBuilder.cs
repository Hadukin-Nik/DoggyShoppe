using System;
using UnityEngine;
using static BuildingsConsts;

public class PlayerBuilder : MonoBehaviour
{
    [SerializeField] private Transform _face;
    [SerializeField] private LayerMask _groundMask;

    [SerializeField] private BuildingsFactory _buildingsFactory;

    [SerializeField] private float _raycastField;
    [SerializeField] private float _rotationSpeed;

    private Floor _floorController;

    private GameObject buildingGO = null;

    private Building _building;
    private bool _isBuilding;

    private BuildingIndificator _buildingIndificator;

    private void Start()
    {
        _floorController = FindObjectOfType<Floor>();
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
        if(_isBuilding && Input.GetKeyDown(KeyCode.RightBracket))
        {
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

            buildingGO = Instantiate(_building._gameBody);
        } else if (_isBuilding && Input.GetKeyDown(KeyCode.LeftBracket))
        {
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

            while(_building == null)
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

        if(Input.GetKeyDown(KeyCode.Z) && Physics.Raycast(_face.position, fwd, out hit, _raycastField) && hit.transform.CompareTag("Building")) { 
            BuildingContoller buildingContollerDelete = hit.transform.GetComponent<BuildingContoller>();
            _floorController.ReleaseBuildingPoints(buildingContollerDelete.getUsedPoints());
            buildingContollerDelete.SetUsedPoints(null);

            Destroy(hit.transform.gameObject);
        }
        else if (_building == null && Input.GetKeyDown(KeyCode.Q) && !_isBuilding && Physics.Raycast(_face.position, fwd, out hit, _raycastField, _groundMask)) {
            _floorController.CreateBuildingMap();
            _building = _buildingsFactory.Get(BuildingIndificator.Desk1);
            buildingGO = Instantiate(_building._gameBody, transform.position, transform.rotation);

            _buildingIndificator = BuildingIndificator.Desk1;
            _isBuilding = true;
        }
        else if (_isBuilding && Input.GetKeyDown(KeyCode.Q) 
            && _floorController.IsItPossibleToBuild(buildingContoller)){
            buildingGO.GetComponentInChildren<BoxCollider>().isTrigger = false;
            _floorController.TryToBuild(buildingContoller);
            _floorController.DestroyBuildingMap();
            _building = null;
            _isBuilding = false;

        } else if (_isBuilding && Input.GetKeyDown(KeyCode.Escape)) {
            _floorController.DestroyBuildingMap();

            _isBuilding = false;
            Destroy(buildingGO);

            _building = null;
        }
    }
}
