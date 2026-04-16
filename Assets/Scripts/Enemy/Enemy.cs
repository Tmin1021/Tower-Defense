using System;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data; // read only data
    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;
    private Path _currentPath;
    private Vector3 _targetPosition;
    private int _currentWaypoint;
    private float _lives;
    private float _maxLives;

    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;

    private bool _hasBeenCounted = false;

    void Awake()
    {
        _currentPath = GameObject.Find("Path1").GetComponent<Path>();
        _healthBarOriginalScale = healthBar.localScale;
    }

    void OnEnable()
    {
        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);
        // _lives = data.lives; // reset the lives of new enemy
        // UpdateHealthBar(); // reset the health bar of new enemy
    }

    void Update()
    { 
        if(_hasBeenCounted) return; 
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);

        float relativeDistance = (transform.position - _targetPosition).magnitude;

        if(relativeDistance < 0.1f)
        {    
            if(_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }
            else // reached last waypoint
            {
                _hasBeenCounted = true; // enemy already destroyed
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            } 
        }
    }

    public void TakeDamage(float damage)
    {
        if(_hasBeenCounted) return; 

        _lives -= damage;
        _lives = Math.Max(0, _lives);
        UpdateHealthBar();

        if(_lives <= 0)
        {
            _hasBeenCounted = true; // enemy already destroyed
            gameObject.SetActive(false);
            OnEnemyDestroyed?.Invoke(this);
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercent = _lives / _maxLives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }

    public void Initialize(float healthMultiplier)
    {
        _maxLives = data.lives * healthMultiplier;
        _lives = _maxLives;
        _hasBeenCounted = false; // reset the pooled object;
        UpdateHealthBar();
    }
}
