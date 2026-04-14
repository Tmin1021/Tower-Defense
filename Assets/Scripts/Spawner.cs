using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    private float _spawnTimer;
    private float _spawnInterval  = 1f;
    // public GameObject Prefab;
    public ObjectPooler pool;

    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if(_spawnTimer < 0)
        {
            _spawnTimer = _spawnInterval;
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        GameObject spawnedObject = pool.GetPooledObject();
        spawnedObject.transform.position = transform.position;
        spawnedObject.SetActive(true);
    }
}
