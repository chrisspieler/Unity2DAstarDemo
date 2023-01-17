using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public event EventHandler OnPathfinderUpdated;

    public List<EnemyController> _enemies;

    private PathFinder _pathfinder;
    private NodeGraph _nodes;

    public float PathfinderUpdateInterval = 2f;
    private float _pathfinderUpdateProgress;

    private void Start()
    {
        GameController.Current.OnEnemySpawned += (s, e) => { AddEnemy(e); };
        GameController.Current.OnGameOver += (s, e) => { _enemies.Clear(); };
        UpdatePathfinder();
    }

    void AddEnemy(EnemyController enemy)
    {
        _enemies.Add(enemy);
        UpdateEnemyPath(enemy);
    }

    private void UpdatePathfinder()
    {
        if (!GameController.Current || !GameController.Current.Player)
        {
            return;
        }
        var player = GameController.Current.Player;
        _nodes = GameController.Current.Nodes;
        var playerCoords = _nodes.NavGrid.WorldToCell(player.transform.position);
        var playerNode = _nodes.Nodes[playerCoords];
        _pathfinder = _nodes.CreatePathFinder(playerNode);
        OnPathfinderUpdated?.Invoke(this, null);
    }

    public void UpdateEnemyPath(EnemyController enemy)
    {
        if (!_nodes || _pathfinder == null || !enemy)
        {
            return;
        }
        var enemyCoords = _nodes.NavGrid.WorldToCell(enemy.transform.position);
        if (!_nodes.Nodes.ContainsKey(enemyCoords))
        {
            return;
        }
        NavNode startNode = _nodes.Nodes[enemyCoords];
        List<Vector2> foundPath;
        if (!_pathfinder.FindPath(startNode, out foundPath))
        {
            throw new InvalidOperationException($"No path to player from {startNode.WorldPosition} could be found");
        }
        enemy.CurrentPath = foundPath;
    }

    void Update()
    {
        _pathfinderUpdateProgress -= Time.deltaTime;
        if (_pathfinderUpdateProgress <= 0)
        {
            UpdatePathfinder();
            _pathfinderUpdateProgress = PathfinderUpdateInterval + _pathfinderUpdateProgress;
        }
    }
}
