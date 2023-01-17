using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public const string ENEMY_TAG = "enemy";
    public const string ENEMY_SPAWNER_TAG = "spawner";

    public int TimedScoreAwardValue = 5;
    public float TimedScoreAwardInterval = 0.5f;
    private float _timedScoreAwardProgress;

    public float EnemySpawnInterval = 3f;
    private float _enemySpawnProgress;

    public Vector2 PlayerSpawnPosition;

    public bool IsGameActive => !GameOverElements.activeSelf;

    public GameObject Player;
    public GameObject PlayerPrefab;
    public List<GameObject> Enemies;
    public List<GameObject> EnemySpawners;
    public GameObject EnemyPrefab;
    public GameObject GameOverElements;
    public NodeGraph Nodes;
    public NavigationManager Navigation;

    public event EventHandler<int> OnScoreUpdated;
    public event EventHandler<EnemyController> OnEnemySpawned;
    public event EventHandler<EnemyController> OnEnemyDestroyed;
    public event EventHandler OnGameStart;
    public event EventHandler OnGameOver;

    public static GameController Current;

    public int Score
    {
        get => _score;
        private set 
        {
            _score = value;
            OnScoreUpdated?.Invoke(this, _score);
        }
    }
    private int _score;

    void Awake()
    {
        if (Current)
        {
            Debug.LogWarning("Warning: Multiple GameControllers in existence!");
        }
        Current = this;
    }

    private void Start()
    {
        EnemySpawners.AddRange(GameObject.FindGameObjectsWithTag(ENEMY_SPAWNER_TAG));
        NewGame();
    }

    private void Update()
    {
        if (!IsGameActive)
        {
            return;
        }
        _timedScoreAwardProgress -= Time.deltaTime;
        if (_timedScoreAwardProgress <= 0)
        {
            AddScore(TimedScoreAwardValue);
            _timedScoreAwardProgress = TimedScoreAwardInterval + _timedScoreAwardProgress;
        }
        _enemySpawnProgress -= Time.deltaTime;
        if (_enemySpawnProgress <= 0)
        {
            SpawnEnemy(GetEnemySpawnPosition());
            _enemySpawnProgress = EnemySpawnInterval + _enemySpawnProgress;
        }
    }

    private Vector2 GetPlayerSpawnPosition()
    {
        return PlayerSpawnPosition;
    }

    /// <summary>
    /// Returns the position of a random enemy spawner, excluding the spawner that is closest to the player.
    /// </summary>
    private Vector2 GetEnemySpawnPosition()
    {
        var allSpawners = EnemySpawners
            .OrderBy(
                p => Vector3.Distance(p.transform.position, Player.transform.position))
            .ToList();
        GameObject closestSpawner = allSpawners.First();
        allSpawners.Remove(closestSpawner);
        var randomSpawnerIndex = new System.Random().Next(allSpawners.Count);
        return allSpawners[randomSpawnerIndex].transform.position;
    }

    public void SpawnPlayer(Vector2 position)
    {
        if (Player)
        {
            Destroy(Player);
            Player = null;
        }
        Player = Instantiate(PlayerPrefab);
        Player.transform.position = position;
        Debug.Log($"Spawned {Player.name} at {position}");
    }

    public void SpawnEnemy(Vector2 position)
    {
        var enemy = Instantiate(EnemyPrefab);
        enemy.transform.position = position;
        Enemies.Add(enemy);
        var enemyController = enemy.GetComponent<EnemyController>();
        OnEnemySpawned?.Invoke(this, enemyController);
        Debug.Log($"Spawned {enemy.name} at {position}");
    }

    public void DestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
        Enemies.Remove(enemy);
    }

    public void DestroyAllEnemies()
    {
        Enemies.ToList().ForEach(p => DestroyEnemy(p));
    }

    public void AddScore(int score)
    {
        if (score <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be above 0");
        }
        Score += score;
    }

    public void NewGame()
    {
        GameOverElements.SetActive(false);
        Score = 0;
        _timedScoreAwardProgress = TimedScoreAwardInterval;
        _enemySpawnProgress = EnemySpawnInterval;
        SpawnPlayer(GetPlayerSpawnPosition());
        DestroyAllEnemies();
        SpawnEnemy(GetEnemySpawnPosition());
        OnGameStart?.Invoke(this, null);
    }

    public void GameOver()
    {
        GameOverElements.SetActive(true);
        OnGameOver?.Invoke(this, null);
    }
}
