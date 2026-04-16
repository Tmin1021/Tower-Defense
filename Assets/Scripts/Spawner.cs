using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    public static event Action<int> OnWaveChanged;
    [SerializeField] WaveData[] waves;
    private int _currentWaveIndex = 0;
    private WaveData _currentWave => waves[_currentWaveIndex];
    private float _spawnTimer;
    private int _spawnCounter = 0;
    private int _waveCounter = 0;
    private int _enemiesRemoved = 0;

    private float _timeBetweenWaves = 1f;
    private float _waveCooldown;
    private bool _isBetweenWaves = false;
    
    [SerializeField] private ObjectPooler orcPool;
    [SerializeField] private ObjectPooler dragonPool;
    [SerializeField] private ObjectPooler kaijuPool;

    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

    void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            {EnemyType.Orc, orcPool},
            {EnemyType.Dragon, dragonPool},
            {EnemyType.Kaiju, kaijuPool}
        };
    }

    void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }
    void Start()
    {
        OnWaveChanged?.Invoke(_waveCounter);
    }

    void Update()
    {
        if(_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if(_waveCooldown <= 0)
            {
                _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length; // continue to next wave 
                _waveCounter++;
                OnWaveChanged?.Invoke(_waveCounter);
                _spawnCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = 0; // immediately spawn the next wave
                _isBetweenWaves = false;
            }
        }
        else
        {    
            _spawnTimer -= Time.deltaTime;
            if(_spawnTimer <= 0 && _spawnCounter < _currentWave.enemiesPerWave)
            {
                _spawnTimer = _currentWave.spawInterval;
                SpawnEnemy();
                _spawnCounter++;
            }
            else if(_spawnCounter >= _currentWave.enemiesPerWave &&
                     _enemiesRemoved >= _currentWave.enemiesPerWave) // if the enemies were cleared all
            {
                _isBetweenWaves = true;
                _waveCooldown = _timeBetweenWaves;
            }
        }
    }

    private void SpawnEnemy()
    {
        if(_poolDictionary.TryGetValue(_currentWave.enemyType, out var pool))
        {    
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;

            float healthMultiplier = 1f + (_waveCounter * 0.1f); // +10% each wave
            Enemy enemy = spawnedObject.GetComponent<Enemy>();
            enemy.Initialize(healthMultiplier);

            spawnedObject.SetActive(true);
        }
    }

    private void HandleEnemyReachedEnd(EnemyData data) // do it when enemy reached ends
    {
        _enemiesRemoved++;

        // Debug.Log(data.name);
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesRemoved++;
    }
}
