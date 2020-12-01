using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public GameObject pooledObject;
    public int pooledAmount;
    private List<GameObject> _pooledObjects;

	private void Start ()
    {
        _pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.transform.SetParent(transform, false);
            obj.SetActive(false);
            _pooledObjects.Add(obj);
        }
	}
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < _pooledObjects.Count; i++)
        {
            if (!_pooledObjects[i].activeInHierarchy)
            {
                return _pooledObjects[i];
            }
        }
        GameObject obj = Instantiate(pooledObject);
        obj.transform.SetParent(transform, false);
        obj.SetActive(false);
        _pooledObjects.Add(obj);
        return obj;
    }
}
