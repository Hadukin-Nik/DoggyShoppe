using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private GameObject _greenDebugPlane;
    [SerializeField] private GameObject _redDebugPlane;

    [SerializeField] private Transform _pointStart;

    [SerializeField] private Vector3 _size;

    [SerializeField] private float _matrixDivisionUnit = 0.5f;

    private FloorController _buildingMatrixController;
    private PathFindingController _pathFindingController;
    private DebugBuildingLayout _debugBuildingLayout;

    private BuildingsHandler _buildingsHandler;

    //OZ axis (local)
    private Vector3 _correctForwardVector;
    //OX axis (local)
    private Vector3 _correctRightVector;

    //First value by OX, second value by OZ axis
    private (int, int) _realMatrixSize;
    void Start()
    {
        _correctForwardVector = _pointStart.forward * _matrixDivisionUnit;
        _correctRightVector = _pointStart.right * _matrixDivisionUnit;

        _realMatrixSize = ((int)(_size.x / _matrixDivisionUnit), (int)(_size.z / _matrixDivisionUnit));

        _buildingMatrixController = new FloorController(_realMatrixSize, _pointStart, _correctForwardVector, _correctRightVector, _matrixDivisionUnit);
        _pathFindingController = new PathFindingController(_realMatrixSize, _correctForwardVector, _correctRightVector, _pointStart.position);
        _debugBuildingLayout = new DebugBuildingLayout(_greenDebugPlane, _redDebugPlane, _pointStart, _realMatrixSize, _correctForwardVector, _correctRightVector);
    }


    public void SetBuildingsHandler(BuildingsHandler buildingsHandler)
    {
        _buildingsHandler = buildingsHandler;
    }

    //PathFinding methods

    public List<Vector3> GetWayToCertainBuilding(Vector3 startPoint, BuildingContoller buildingContoller)
    {
        List<(BuildingContoller, List<(int, int)>)> buf = _buildingsHandler.GetAll();
        if (buf.Count > 0)
            return _pathFindingController.GetWay(_buildingMatrixController.getBuildingMatrix(), _buildingMatrixController.fromGlobalToMatrix(startPoint), buf[buf.Count - 1].Item2);

        return null;
    }
    public List<Vector3> GetWayToLast(Vector3 startPoint)
    {
        List<(BuildingContoller, List<(int, int)>)> buf = _buildingsHandler.GetAll();
        if (buf.Count > 0)
            return _pathFindingController.GetWay(_buildingMatrixController.getBuildingMatrix(), _buildingMatrixController.fromGlobalToMatrix(startPoint), buf[buf.Count - 1].Item2);

        return null;
    }

    public List<Vector3> GetWayToRandom(Vector3 startPoint)
    {
        List<(BuildingContoller, List<(int, int)>)> buf = _buildingsHandler.GetAll();
        if (buf.Count > 0)
            return _pathFindingController.GetWay(_buildingMatrixController.getBuildingMatrix(), _buildingMatrixController.fromGlobalToMatrix(startPoint), buf[Random.Range(0, buf.Count)].Item2);

        return null;
    }

    //BuildingMatrixController methods
    public bool IsItPossibleToBuild(BuildingContoller buildingContoller)
    {
        return _buildingMatrixController.IsItPossibleToBuild(buildingContoller);
    }

    public List<(int, int)> TryToBuild(BuildingContoller buildingContoller)
    {
        return _buildingMatrixController.TryToBuild(buildingContoller);
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
