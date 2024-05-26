using System;
using System.Collections.Generic;

public class BuildingModel : IEquatable<BuildingModel>, IComparable<BuildingModel>
{
    private int _id;
    private List<(int, int)> _points;
    private ItemsConsts.ItemIndificator _itemType;
    private int _count;

    public BuildingModel(int id, List<(int, int)> points)
    {
        _id = id;
        _points = points;
    }

    public void setType(ItemsConsts.ItemIndificator itemIndificator)
    {
        _itemType = itemIndificator;
    }

    public void setCount(int count)
    {
        _count = count;
    }

    public bool Equals(BuildingModel other)
    {
        return _id == other._id;
    }

    public int CompareTo(BuildingModel other)
    {
        return _id.CompareTo(other._id);
    }
}
