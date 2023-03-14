using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using Pathfinding._Scripts.Grid.Scriptables;
using Pathfinding._Scripts.Units;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Pathfinding._Scripts.Grid {
    public class GridManager : MonoBehaviour 
    {
        public static GridManager Instance;
        
        [SerializeField] private Sprite _playerSprite, _goalSprite;
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private ScriptableGrid _scriptableGrid;
        [SerializeField] private bool _drawConnections;
        [SerializeField] private float delayBetweenSearch;
        public Dictionary<Vector2, NodeBase> Tiles { get; private set; }

        private NodeBase _playerNodeBase, _goalNodeBase;
        private Unit _spawnedPlayer, _spawnedGoal;

        void Awake() => Instance = this;

        private void Start() {
            Tiles = _scriptableGrid.GenerateGrid();
         
            foreach (var tile in Tiles.Values) tile.CacheNeighbors();

            SpawnUnits();
            NodeBase.OnHoverTile += OnTileHover;
        }

        private void OnDestroy() => NodeBase.OnHoverTile -= OnTileHover;

        private void OnTileHover(NodeBase nodeBase) {
            _goalNodeBase = nodeBase;
            _spawnedGoal.transform.position = _goalNodeBase.Coords.Pos;

            foreach (var t in Tiles.Values) t.RevertTile();
            
        }

        void SpawnUnits() {
            _playerNodeBase = Tiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Value;
            _spawnedPlayer = Instantiate(_unitPrefab, _playerNodeBase.Coords.Pos, Quaternion.identity);
            _spawnedPlayer.Init(_playerSprite);

            _spawnedGoal = Instantiate(_unitPrefab, new Vector3(50, 50, 50), Quaternion.identity);
            _spawnedGoal.Init(_goalSprite);
        }

        public NodeBase GetTileAtPosition(Vector2 pos) => Tiles.TryGetValue(pos, out var tile) ? tile : null;

        private void OnDrawGizmos() {
            if (!Application.isPlaying || !_drawConnections) return;
            Gizmos.color = Color.red;
            foreach (var tile in Tiles) {
                if (tile.Value.Connection == null) continue;
                Gizmos.DrawLine((Vector3)tile.Key + new Vector3(0, 0, -1), (Vector3)tile.Value.Connection.Coords.Pos + new Vector3(0, 0, -1));
            }
        }
        [ButtonEditor]
        public void RecreateMatrix()
        {
            SceneManager.LoadScene("Main");

        }
        [ButtonEditor]
        public void AStar()
        {
            ResetVisual();
            StartCoroutine(Pathfinding.AStar(_playerNodeBase, _goalNodeBase, delayBetweenSearch));
            
        }
        [ButtonEditor]
        public void Dijkstra()
        {
            ResetVisual();
            StartCoroutine(Pathfinding.Dijkstra(_playerNodeBase, _goalNodeBase, delayBetweenSearch));
        }
        [ButtonEditor]
        public void DFS()
        {
            ResetVisual();
            StartCoroutine(Pathfinding.DFS(_playerNodeBase, _goalNodeBase, delayBetweenSearch));
        }
        [ButtonEditor]
        public void BFS()
        {
            ResetVisual();
            StartCoroutine(Pathfinding.BFS(_playerNodeBase, _goalNodeBase, delayBetweenSearch));
        }

        private void ResetVisual()
        {
            foreach (var t in Tiles.Values) t.RevertTile();
            StopAllCoroutines();
        }
        
    }
}