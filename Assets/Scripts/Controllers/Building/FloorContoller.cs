using System.Collections.Generic;
using UnityEngine;

public class FloorController
{
    private KeyValuePair<int, int> _matrixSize;

    private Vector3 _pointStart;


    private bool[,] _buildingMatrix;

    private float[,] _transformationMatrix;
    private float[,] _transformedGlobalPivot;

    private float _divisionUnit;

    //OZ axis (local)
    private Vector3 _correctForwardVector;
    //OX axis (local)
    private Vector3 _correctRightVector;


    public FloorController(KeyValuePair<int, int> realMatrixSize, Transform pointStart, Vector3 correctFwdVector, Vector3 correctRghtVector, float divisionUnit)
    {
        _matrixSize = realMatrixSize;
        _correctForwardVector = correctFwdVector;
        _correctRightVector = correctRghtVector;

        _divisionUnit = divisionUnit;
        _pointStart = pointStart.position;
        //False means - no building on that point
        _buildingMatrix = new bool[realMatrixSize.Key, realMatrixSize.Value];

        _transformationMatrix = new float[2, 2];
        _transformedGlobalPivot = new float[2, 1];

        Vector3 right = pointStart.right;
        Vector3 forward = pointStart.forward;

        float detA = (right.x * forward.z - right.z * forward.x);

        _transformationMatrix[0, 0] = forward.z / detA;
        _transformationMatrix[0, 1] = (-forward.x) / detA;
        _transformationMatrix[1, 0] = (-right.z) / detA;
        _transformationMatrix[1, 1] = (right.x) / detA;

        _transformedGlobalPivot[0, 0] = -(_transformationMatrix[0, 0] * pointStart.position.x + _transformationMatrix[0, 1] * pointStart.position.z);
        _transformedGlobalPivot[1, 0] = -(_transformationMatrix[1, 0] * pointStart.position.x + _transformationMatrix[1, 1] * pointStart.position.z);
    }


    public Vector3 fromMatrixToGlobal(KeyValuePair<int, int> point)
    {
        return point.Value * _correctForwardVector + point.Key * _correctRightVector + _pointStart;
    }

    public KeyValuePair<int, int> fromGlobalToMatrix(Vector3 point)
    {
        float xT = _transformationMatrix[0, 0] * point.x + _transformationMatrix[0, 1] * point.z + _transformedGlobalPivot[0, 0];
        float zT = _transformationMatrix[1, 0] * point.x + _transformationMatrix[1, 1] * point.z + _transformedGlobalPivot[1, 0];

        if (xT < 0 || zT < 0 || xT > _matrixSize.Key * _divisionUnit || zT > _matrixSize.Value * _divisionUnit)
        {
            return findClosestPointInPeremeter(point);
        }
        else
        {
            return new KeyValuePair<int, int>((int)(xT / _divisionUnit), (int)(zT / _divisionUnit));
        }
    }

    //You must pass a point in general world space
    public bool IsInMatrix(Vector3 point)
    {
        //given point will be named M
        //ab
        //cd
        Vector3 a = _pointStart;
        Vector3 b = _pointStart + _correctForwardVector * _matrixSize.Value;
        Vector3 d = _pointStart + _correctRightVector * _matrixSize.Key;

        Vector3 am = point - a;
        Vector3 ab = b - a;
        Vector3 ad = d - a;

        return (0 < scal(am, ab) && scal(am, ab) < scal(ab, ab)) && (0 < scal(am, ad) && scal(am, ad) < scal(ad, ad));
    }


    public bool IsItPossibleToBuild(BuildingContoller buildingContoller)
    {
        return isItPossibleToBuild(buildingContoller.GetTransform(), buildingContoller.GetSize(), false).Key;
    }

    public List<KeyValuePair<int, int>> TryToBuild(BuildingContoller buildingContoller)
    {
        return isItPossibleToBuild(buildingContoller.GetTransform(), buildingContoller.GetSize(), true).Value;
    }

    public bool[,] getBuildingMatrix()
    {
        return _buildingMatrix;
    }

    private KeyValuePair<bool, List<KeyValuePair<int, int>>> isItPossibleToBuild(Transform buildingTransform, Vector3 buildingSize, bool buildImmediately)
    {
        float exp = 0.1f;
        List<Vector3> pointsOnMatrix = new List<Vector3>();

        for(int i = 0; i < buildingSize.x / _divisionUnit / exp; i++)
        {
            for(int j = 0; j < buildingSize.z / _divisionUnit / exp; j++)
            {
                float deltaX = i * _divisionUnit * exp;
                float deltaZ = j * _divisionUnit * exp;

                Vector3 nv = deltaX * buildingTransform.right + deltaZ * buildingTransform.forward - buildingTransform.right * buildingSize.x / 2 - buildingTransform.forward * buildingSize.z / 2;

                float xG = (nv.x + buildingTransform.position.x);
                float zG = (nv.z + buildingTransform.position.z);

                float xT = _transformationMatrix[0, 0] * xG + _transformationMatrix[0, 1] * zG + _transformedGlobalPivot[0, 0];
                float zT = _transformationMatrix[1, 0] * xG + _transformationMatrix[1, 1] * zG + _transformedGlobalPivot[1, 0];

                Vector3 point = new Vector3(xT / _divisionUnit, 0, zT / _divisionUnit);

                if(xT <= 0 || zT <= 0 || xT >= _matrixSize.Key * _divisionUnit || zT >= _matrixSize.Value * _divisionUnit || _buildingMatrix[(int)(point.x), (int)(point.z)])
                {
                    return new KeyValuePair<bool, List<KeyValuePair<int, int>>>(false, null);
                } else
                {
                    pointsOnMatrix.Add(point);
                }
            }
        }

        if(!buildImmediately)
        {
            return new KeyValuePair<bool, List<KeyValuePair<int, int>>>(true, null);
        }

        List<KeyValuePair<int, int>> map = new List<KeyValuePair<int, int>>();

        foreach(Vector3 point in pointsOnMatrix)
        {
            map.Add(new KeyValuePair<int, int>((int)(point.x), (int)(point.z)));
            _buildingMatrix[(int)(point.x), (int)(point.z)] = true;
        }
      
        return new KeyValuePair<bool, List<KeyValuePair<int, int>>>(true, map);
    }

    private double scal(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    private KeyValuePair<int, int> findClosestPointInPeremeter(Vector3 point)
    {
        KeyValuePair<int, int> ans = new KeyValuePair<int, int>(0, 0);
        Vector3 ansP = _pointStart;
        for (int i = 0; i < _matrixSize.Key; i++)
        {
            KeyValuePair<int, int> pointB;
            if (i == 0 || i == _matrixSize.Key - 1)
            {
                for (int j = 0; j < _matrixSize.Value; j++)
                {
                    pointB = new KeyValuePair<int, int>(i, j);
                    Vector3 point1 = fromMatrixToGlobal(pointB);

                    if ((point - point1).sqrMagnitude < (point - ansP).sqrMagnitude)
                    {
                        ans = pointB;
                        ansP = point1;
                    }
                }
            }
            else
            {
                pointB = new KeyValuePair<int, int>(i, 0);
                Vector3 point1 = fromMatrixToGlobal(pointB);

                if ((point - point1).sqrMagnitude < (point - ansP).sqrMagnitude)
                {
                    ans = pointB;
                    ansP = point1;
                }

                pointB = new KeyValuePair<int, int>(i, Mathf.Abs(_matrixSize.Value - 1));
                Vector3 point2 = fromMatrixToGlobal(new KeyValuePair<int, int>(i, Mathf.Abs(_matrixSize.Value - 1)));

                if ((point - point2).sqrMagnitude < (point - ansP).sqrMagnitude)
                {
                    ans = pointB;
                    ansP = point2;
                }
            }
        }
        return ans;
    }




}
