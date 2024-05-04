using UnityEngine;

public class BuildingContoller : MonoBehaviour
{
    private Building _data;
    

    public Vector3 GetSize()
    {
        return _data._size;
    }

    public void SetData(Building data) { 
        _data = data;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
