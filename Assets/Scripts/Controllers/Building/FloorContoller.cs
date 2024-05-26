using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorController
{
    private (int, int) _matrixSize;

    private Vector3 _pointStart;


    private bool[,] _buildingMatrix;

    private float[,] _transformationMatrix;
    private float[,] _transformedGlobalPivot;

    private float _divisionUnit;

    //OZ axis (local)
    private Vector3 _correctForwardVector;
    //OX axis (local)
    private Vector3 _correctRightVector;


    public FloorController((int, int) realMatrixSize, Transform pointStart, Vector3 correctFwdVector, Vector3 correctRghtVector, float divisionUnit)
    {
        _matrixSize = realMatrixSize;
        _correctForwardVector = correctFwdVector;
        _correctRightVector = correctRghtVector;

        _divisionUnit = divisionUnit;
        _pointStart = pointStart.position;
        //False means - no building on that point
        _buildingMatrix = new bool[realMatrixSize.Item1, realMatrixSize.Item2];

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


    public Vector3 fromMatrixToGlobal((int, int) point)
    {
        return point.Item2 * _correctForwardVector + point.Item1 * _correctRightVector + _pointStart;
    }

    public (int, int) fromGlobalToMatrix(Vector3 point)
    {
        float xT = _transformationMatrix[0, 0] * point.x + _transformationMatrix[0, 1] * point.z + _transformedGlobalPivot[0, 0];
        float zT = _transformationMatrix[1, 0] * point.x + _transformationMatrix[1, 1] * point.z + _transformedGlobalPivot[1, 0];

        if (xT < 0 || zT < 0 || xT > _matrixSize.Item1 * _divisionUnit || zT > _matrixSize.Item2 * _divisionUnit)
        {
            return findClosestPointInPeremeter(point);
        }
        else
        {
            return ((int)(xT / _divisionUnit), (int)(zT / _divisionUnit));
        }
    }

    //You must pass a point in general world space
    public bool IsInMatrix(Vector3 point)
    {
        //given point will be named M
        //ab
        //cd
        Vector3 a = _pointStart;
        Vector3 b = _pointStart + _correctForwardVector * _matrixSize.Item2;
        Vector3 d = _pointStart + _correctRightVector * _matrixSize.Item1;

        Vector3 am = point - a;
        Vector3 ab = b - a;
        Vector3 ad = d - a;

        return (0 < scal(am, ab) && scal(am, ab) < scal(ab, ab)) && (0 < scal(am, ad) && scal(am, ad) < scal(ad, ad));
    }


    public bool IsItPossibleToBuild(BuildingContoller buildingContoller)
    {
        return isItPossibleToBuild((buildingContoller.GetTransform().position, buildingContoller.GetTransform().rotation), buildingContoller.GetSize(), false).Item1;
    }

    public bool IsItPossibleToBuild((Vector3, Quaternion) transform, Vector3 size)
    {
        return isItPossibleToBuild(transform, size, false).Item1;
    }

    public List<(int, int)> TryToBuild(BuildingContoller buildingContoller)
    {
        return isItPossibleToBuild((buildingContoller.GetTransform().position, buildingContoller.GetTransform().rotation), buildingContoller.GetSize(), true).Item2;
    }

    public List<(int, int)> TryToBuild((Vector3, Quaternion) transform, Vector3 size)
    {
        return isItPossibleToBuild(transform, size, true).Item2;
    }

    public bool[,] getBuildingMatrix()
    {
        return _buildingMatrix;
    }

    private (bool, List<(int, int)>) isItPossibleToBuild((Vector3, Quaternion) buildingTransform, Vector3 buildingSize, bool buildImmediately)
    {
        float exp = 0.1f;
        HashSet<(int, int)> pointsOnMatrix = new HashSet<(int, int)>();

        Vector3 forwardDirection = buildingTransform.Item2 * Vector3.forward;
        Vector3 rightDirection = buildingTransform.Item2 * Vector3.right;

        for (int i = 0; i < buildingSize.x / _divisionUnit / exp; i++)
        {
            for(int j = 0; j < buildingSize.z / _divisionUnit / exp; j++)
            {
                float deltaX = i * _divisionUnit * exp;
                float deltaZ = j * _divisionUnit * exp;

                Vector3 nv = deltaX * rightDirection + deltaZ * forwardDirection - rightDirection * buildingSize.x / 2 - forwardDirection * buildingSize.z / 2;

                float xG = (nv.x + buildingTransform.Item1.x);
                float zG = (nv.z + buildingTransform.Item1.z);

                float xT = _transformationMatrix[0, 0] * xG + _transformationMatrix[0, 1] * zG + _transformedGlobalPivot[0, 0];
                float zT = _transformationMatrix[1, 0] * xG + _transformationMatrix[1, 1] * zG + _transformedGlobalPivot[1, 0];
                (int, int) point = ((int)(xT / _divisionUnit), (int)(zT / _divisionUnit));

                if (xT <= 0 || zT <= 0 || xT >= _matrixSize.Item1 * _divisionUnit || zT >= _matrixSize.Item2 * _divisionUnit || _buildingMatrix[(int)(point.Item1), (int)(point.Item2)])
                {
                    return (false, null);
                } else
                {
                    pointsOnMatrix.Add(point);
                }
            }
        }

        if(!buildImmediately)
        {
            return(true, null);
        }

        List<(int, int)> map = new List<(int, int)>();

        foreach((int, int) point in pointsOnMatrix)
        {
            map.Add(((int)(point.Item1), (int)(point.Item2)));
            _buildingMatrix[(int)(point.Item1), (int)(point.Item2)] = true;
        }
      
        return (true, map);
    }

    private double scal(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    public (int, int) findClosestPointInPeremeter(Vector3 point)
    {
       (int, int) ans = (0, 0);
        Vector3 ansP = _pointStart;
        for (int i = 0; i < _matrixSize.Item1; i++)
        {
            (int, int) pointB;
            if (i == 0 || i == _matrixSize.Item1 - 1)
            {
                for (int j = 0; j < _matrixSize.Item2; j++)
                {
                    pointB = (i, j);
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
                pointB = (i, 0);
                Vector3 point1 = fromMatrixToGlobal(pointB);

                if ((point - point1).sqrMagnitude < (point - ansP).sqrMagnitude)
                {
                    ans = pointB;
                    ansP = point1;
                }

                pointB = (i, Mathf.Abs(_matrixSize.Item2 - 1));
                Vector3 point2 = fromMatrixToGlobal((i, Mathf.Abs(_matrixSize.Item2 - 1)));

                if ((point - point2).sqrMagnitude < (point - ansP).sqrMagnitude)
                {
                    ans = pointB;
                    ansP = point2;
                }
            }
        }
        return ans;
    }

    public void ReleasePoints(List<(int, int)> points)
    {
        foreach((int, int) point in points)
        {
            _buildingMatrix[point.Item1, point.Item2] = false;
        }
    }
}
