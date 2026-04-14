using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private float poolSize = 5;
    [SerializeField] private GameObject prefab;

    private List<GameObject> _pool;

    void Start()
    {
        _pool = new List<GameObject>();

        for(int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    } 

    public GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        _pool.Add(obj);
        return obj;
    }

    public GameObject GetPooledObject()
    {
        foreach(var obj in _pool)
        {
            if(!obj.activeSelf)
            {
                return obj;
            }
        }

        return CreateNewObject();
    }
}
