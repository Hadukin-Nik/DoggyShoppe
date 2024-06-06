using System.Collections.Generic;
using UnityEngine;

public class PathFindingController
{
    private (int, int) _size;

    //OZ axis (local)
    private Vector3 _correctForwardVector;
    //OX axis (local)
    private Vector3 _correctRightVector;

    private Vector3 _pointStart;
    public PathFindingController((int, int) size, Vector3 correctFwdVector, Vector3 correctRghtVector, Vector3 pointStart)
    {
        _size = size;
        _correctForwardVector = correctFwdVector;
        _correctRightVector = correctRghtVector;
        _pointStart = pointStart;
    }

    private Vector3 toPointFromMatrix(float i, float j)
    {
        return i * _correctRightVector + j * _correctForwardVector;
    }

    private bool inList((int, int) startPoint, List<(int, int)> endPoints)
    {
        foreach(var i in endPoints)
        {
            if(i == startPoint)
            {
                return true;
            }
        }

        return false;
    }

    private bool isNear((int, int) startPoint, List<(int, int)> points)
    {
        List<(int, int)> nearPoint = new List<(int, int)> ();
        HashSet<(int, int)> pointsCheck = new HashSet<(int, int)>();

        foreach(var point in points)
        {
            pointsCheck.Add(point);
        }

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                (int, int) kb = (i + startPoint.Item1, j + startPoint.Item2);
                if (isInBounds(kb.Item1, kb.Item2) && pointsCheck.Contains(kb))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<Vector3> GetWay(bool[,] buildingMatrix, (int, int) startPoint, List<(int, int)> endPoints)
    { 
        
        //point distination
        (int, int) constPoint = endPoints[endPoints.Count - 1];

        //Heap for search points
        SortedSet<int> distances = new SortedSet<int>();
        Dictionary<int, Stack<(int, int)>> points = new Dictionary<int, Stack<(int, int)>>();

        //creating start point
        int distination = (int)(toPointFromMatrix(startPoint.Item1 - constPoint.Item1, startPoint.Item2 - constPoint.Item2)).sqrMagnitude;
        distances.Add(distination);
        points.Add(distination, new Stack<(int, int)>());
        points.GetValueOrDefault(distination).Push(startPoint);

        //init parents, checked points
        (int, int)[,] parents = new (int, int)[_size.Item1, _size.Item2];
        int[,] pathLength = new int[_size.Item1, _size.Item2];
        bool[,] isChecked = new bool[_size.Item1, _size.Item2];
        for (int i = 0; i < _size.Item1; i++)
        {
            for (int j = 0; j < _size.Item2; j++)
            {
                parents[i, j] = (-1, -1);
                isChecked[i, j] = false;
                pathLength[i, j] = -1;
            }
        }

        //init def for while
        pathLength[startPoint.Item1, startPoint.Item2] = 0;
        (int, int) last = (-1, -1);
        bool isFind = false;
        while (distances.Count > 0 && !isFind)
        {
            int distBuf = distances.Min;
            Stack<(int, int)> pointBuf = points.GetValueOrDefault(distBuf);

            while (pointBuf.Count > 0)
            {
                (int, int) point = pointBuf.Pop();

                if (isChecked[point.Item1, point.Item2]) continue;
                isChecked[point.Item1, point.Item2] = true;
                
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        (int, int) kb = (i + point.Item1, j + point.Item2);
                        if (!isInBounds(kb.Item1, kb.Item2) || isChecked[kb.Item1, kb.Item2] || buildingMatrix[kb.Item1, kb.Item2]) continue;

                        if (parents[kb.Item1, kb.Item2].Item1 == -1 && parents[kb.Item1, kb.Item2].Item2 == -1) parents[kb.Item1, kb.Item2] = point;
                        if (pathLength[kb.Item1, kb.Item2] < 0)
                        {
                            if(i * j == 0)
                            {
                                pathLength[kb.Item1, kb.Item2] = pathLength[point.Item1, point.Item2] + 2;
                            } else
                            {
                                pathLength[kb.Item1, kb.Item2] = pathLength[point.Item1, point.Item2] + 3;
                            }
                        }

                        distination = (int)((int)toPointFromMatrix(kb.Item1 - constPoint.Item1, kb.Item2 - constPoint.Item2).sqrMagnitude * 4 + pathLength[kb.Item1, kb.Item2]);

                        distances.Add(distination);

                        if (!points.ContainsKey(distination))
                        {
                            points.Add(distination, new Stack<(int, int)>());
                        }

                        points.GetValueOrDefault(distination).Push(kb);

                        if (isNear(kb, endPoints))
                        {
                            last = kb;
                            isFind = true;
                        }
                    }
                }
            }

            distances.Remove(distBuf);
        }

        if (isFind)
        {
            Stack<(int, int)> keyValuePairs = new Stack<(int, int)>();
            while (last.Item1 != startPoint.Item1 || last.Item2 != startPoint.Item2)
            {
                keyValuePairs.Push((last.Item1, last.Item2));
                last = parents[last.Item1, last.Item2];
            }

            List<Vector3> ans = new List<Vector3>();
            while (keyValuePairs.Count > 0)
            {
                (int, int) pair = keyValuePairs.Pop();
                ans.Add(toPointFromMatrix(pair.Item1, pair.Item2) + _pointStart);
            }

            return ans;
        }
        return null;
    }
    private bool isInBounds(int i, int j)
    {
        return i >= 0 && i < _size.Item1 && j >= 0 && j < _size.Item2;
    }
    


}
