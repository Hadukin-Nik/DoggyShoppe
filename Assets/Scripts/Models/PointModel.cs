
using System;
using System.ComponentModel;

public class PointModel : IEquatable<PointModel> 
{
    private const double EPS = 0.00001;

    private double _x;
    private double _y;

    public PointModel(double x, double y) { 
        _x = x;
        _y = y;
    }
    
    public bool Equals(PointModel other)
    {
        return Math.Abs(_x - other._x) < EPS && Math.Abs(_y - other._y) < EPS;
    }

    public int CompareTo(PointModel other)
    {
        if(_x.CompareTo(other._x) == 0)
        {
            return _y.CompareTo(other._y);
        } else
        {
            return _x.CompareTo(other._x);
        }
    }
}
