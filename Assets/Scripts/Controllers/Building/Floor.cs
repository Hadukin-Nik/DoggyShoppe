using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private GameObject _greenDebugPlane;
    [SerializeField] private GameObject _redDebugPlane;

    [SerializeField] private Transform _pointStart;
    [SerializeField] private GameObject _exitObject;
    [SerializeField] private Vector3 _size;

    [SerializeField] private float _matrixDivisionUnit = 0.5f;

    private FloorController _buildingMatrixController;
    private PathFindingController _pathFindingController;
    private DebugBuildingLayout _debugBuildingLayout;
    private HashSetNListStructure<ItemHolder> _itemHolders;
    private List<List<(int, int)>> _endPoints;
    private List<(int, int)> _exitPoints;
    //OZ axis (local)
    private Vector3 _correctForwardVector;
    //OX axis (local)
    private Vector3 _correctRightVector;

    //First value by OX, second value by OZ axis
    private (int, int) _realMatrixSize;
    void Start()
    {
        _endPoints = new List<List<(int, int)>>();
        _itemHolders = new HashSetNListStructure<ItemHolder>();

        _correctForwardVector = _pointStart.forward * _matrixDivisionUnit;
        _correctRightVector = _pointStart.right * _matrixDivisionUnit;

        _realMatrixSize = ((int)(_size.x / _matrixDivisionUnit), (int)(_size.z / _matrixDivisionUnit));

        _buildingMatrixController = new FloorController(_realMatrixSize, _pointStart, _correctForwardVector, _correctRightVector, _matrixDivisionUnit);
        _pathFindingController = new PathFindingController(_realMatrixSize, _correctForwardVector, _correctRightVector, _pointStart.position);
        _debugBuildingLayout = new DebugBuildingLayout(_greenDebugPlane, _redDebugPlane, _pointStart, _realMatrixSize, _correctForwardVector, _correctRightVector);

        _exitPoints = new List<(int, int)>
        {
            _buildingMatrixController.findClosestPointInPeremeter(_exitObject.transform.position)
        };
    }

    //PathFinding methods
    public (List<Vector3>, ItemHolder) GetWayToRandom(Vector3 startPoint)
    {
        ItemHolder itemHolder = _itemHolders.GetRandom();
        if (_itemHolders.Size() > 0)
            return (_pathFindingController.GetWay(_buildingMatrixController.getBuildingMatrix(), _buildingMatrixController.fromGlobalToMatrix(startPoint), itemHolder.getPoints()), itemHolder);

        return (null, null);
    }

    public List<Vector3> GetWayToRandomEndPoint(Vector3 startPoint)
    {
        if (_endPoints.Count > 0)
            return _pathFindingController.GetWay(_buildingMatrixController.getBuildingMatrix(), _buildingMatrixController.fromGlobalToMatrix(startPoint), _endPoints[Random.Range(0, _endPoints.Count)]);

        return null;
    }

    public List<Vector3> GetWayToExitPoint(Vector3 startPoint)
    {
        if (_endPoints.Count > 0)
            return _pathFindingController.GetWay(_buildingMatrixController.getBuildingMatrix(), _buildingMatrixController.fromGlobalToMatrix(startPoint), _exitPoints);

        return null;
    }

    //BuildingMatrixController methods
    public bool IsItPossibleToBuild(BuildingContoller buildingContoller)
    {
        return _buildingMatrixController.IsItPossibleToBuild(buildingContoller);
    }
    public bool IsItPossibleToBuild((Vector3, Quaternion) transform, Vector3 size)
    {
        return _buildingMatrixController.IsItPossibleToBuild(transform, size);
    }
    public List<(int, int)> TryToBuild(BuildingContoller buildingContoller)
    {
        List<(int, int)> ans = _buildingMatrixController.TryToBuild(buildingContoller);
        buildingContoller.SetUsedPoints(ans);
        if (ans != null && ans.Count >= 1)
        {
            if(buildingContoller.GetIndificator().Equals(BuildingsConsts.BuildingIndificator.EndPoint))
            {
                _endPoints.Add(ans);
            } 
            else {
                foreach (ItemHolder itemHolder in buildingContoller.GetItemHolders())
                {
                    itemHolder.setPoints(ans);
                    _itemHolders.Add(itemHolder);
                }
            }
        }

        return ans;
    }

    public List<(int, int)> TryToBuild((Vector3, Quaternion)  buildingContoller, Vector3 size)
    {
        List<(int, int)> ans = _buildingMatrixController.TryToBuild(buildingContoller, size);

        return ans;
    }

    public void ReleaseBuildingPoints(List<(int, int)> points)
    {
        if (points == null) return;

        _buildingMatrixController.ReleasePoints(points);
    }

    //DebugBuildingLayout methods
    public void CreateBuildingMap() {
        _debugBuildingLayout.CreateBuildingMap(_buildingMatrixController.getBuildingMatrix());
    }

    public void DestroyBuildingMap()
    {
        _debugBuildingLayout.DestroyBuildingMap();
    }
}
