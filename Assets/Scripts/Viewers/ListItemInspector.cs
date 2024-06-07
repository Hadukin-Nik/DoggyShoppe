#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemFactory))]
public class ListItemInspector : Editor
{
    private ItemFactory _classHolder;
    private SerializedObject _targetObject;
    private SerializedProperty _list;
    private int _listSize;

    void OnEnable()
    {
        _classHolder = (ItemFactory)target;
        _targetObject = new SerializedObject(_classHolder);
        _list = _targetObject.FindProperty("items"); // Find the List in our script and create a refrence of it
    }

    public override void OnInspectorGUI()
    {
        //Update our list
        _targetObject.Update();

        //Choose how to display the list<> Example purposes only
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Resize our list
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Define the list size with a number");
        _listSize = _list.arraySize;
        _listSize = EditorGUILayout.IntField("List Size", _listSize);

        if (_listSize != _list.arraySize)
        {
            while (_listSize > _list.arraySize)
            {
                _list.InsertArrayElementAtIndex(_list.arraySize);
            }
            while (_listSize < _list.arraySize)
            {
                _list.DeleteArrayElementAtIndex(_list.arraySize - 1);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Or add a new item to the List<> with a button
        EditorGUILayout.LabelField("Add a new item with a button");

        if (GUILayout.Button("Add New"))
        {
            _classHolder.Add(new Item());
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Display our list to the inspector window

        for (int i = 0; i < _list.arraySize; i++)
        {
            SerializedProperty MyListRef = _list.GetArrayElementAtIndex(i);
            SerializedProperty MyEnum = MyListRef.FindPropertyRelative("_itemIndificator");
            SerializedProperty MyGameObject = MyListRef.FindPropertyRelative("_gameBody");
            SerializedProperty MyName = MyListRef.FindPropertyRelative("_name");
            SerializedProperty MySize = MyListRef.FindPropertyRelative("_size");

            SerializedProperty MyMaterial = MyListRef.FindPropertyRelative("_boxMaterial");

            // Display the property fields in two ways.

            EditorGUILayout.LabelField("Automatic Field By Property Type");
            EditorGUILayout.PropertyField(MyEnum);
            EditorGUILayout.PropertyField(MyGameObject);
            EditorGUILayout.PropertyField(MyName);
            EditorGUILayout.PropertyField(MySize);
            EditorGUILayout.PropertyField(MyMaterial);


            // Array fields with remove at index
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Remove This Index (" + i.ToString() + ")"))
            {
                _list.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        //Apply the changes to our list
        _targetObject.ApplyModifiedProperties();
    }
}
#endif