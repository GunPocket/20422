using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour { //https://www.youtube.com/watch?v=TeurfjuEIgA

    [Header("Game references")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Camera _camera;

    [Header("Game settings")]
    [SerializeField] private int _width = 3;
    [SerializeField] private int _height = 3;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private float _scoreMultiplier = 1;

    public int WinCondition = 2048;

    [Header("Blocks")]
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private List<BlockSO> _blockTypes;

    [Header("Nodes")]
    [SerializeField] private Node _nodePrefab;

    [Header("Board")]
    [SerializeField] private SpriteRenderer _boardPrfab;

    private List<Node> _nodes;
    private List<Block> _blocks;
    private SpriteRenderer _board;
    private GameState _gameState;
    private int _round;

    private BlockSO GetBlockTypeByValue(int value) => _blockTypes.First(t => t.Value == value);

    private void Start() {
        _scoreManager.Initializer(_uiManager);

        _uiManager.ResetButton.style.visibility = Visibility.Hidden;

        _uiManager.ResetButton.clickable.clicked += delegate {
            ResetGame();
        };

        ChangeState(GameState.GenerateLevel);
    }

    private void ResetGame() {
        _uiManager.ResetButton.style.visibility = Visibility.Hidden;

        foreach (var block in _blocks) {
            Destroy(block.gameObject);
        }
        foreach (var node in _nodes) {
            Destroy(node.gameObject);
        }

        Destroy(_board);

        _blocks = null;
        _nodes = null;

        GenerateGrid();
    }

    private void GenerateGrid() {
        _round = 0;
        _blocks = new List<Block>();

        GenerateNodes();

        var center = new Vector2((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f);

        _camera.gameObject.transform.position = new Vector3(center.x, center.y, -10);

        _board = Instantiate(_boardPrfab, center, Quaternion.identity);
        _board.size = new Vector2(_width, _height);

        ChangeState(GameState.SpawningBlocks);
    }

    private void GenerateNodes() {
        _nodes = new List<Node>();

        for (int i = 0; i < _width; i++) {
            for (int j = 0; j < _height; j++) {
                var node = Instantiate(_nodePrefab, new Vector2(i, j), Quaternion.identity);
                _nodes.Add(node);
            }
        }
    }

    private void ChangeState(GameState newState) {
        _gameState = newState;
        _uiManager.TextGameState.text = $"{_gameState}";

        switch (newState) {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                _uiManager.ResetButton.style.visibility = Visibility.Visible;
                break;
            case GameState.Lose:
                _uiManager.ResetButton.style.visibility = Visibility.Visible;
                break;
            case GameState.Upgrade:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void SpawnBlocks(int ammount) {

        var freeNodes = _nodes.Where(x => x.OccupiedBlock == null).OrderBy(b => UnityEngine.Random.value).ToList();

        foreach (var node in freeNodes.Take(ammount)) {
            SpawnBlock(node, UnityEngine.Random.value > 0.8f ? 4 : 2);
        }

        if (freeNodes.Count() == 1) {
            if (!CheckIfCanMove()) {
                ChangeState(GameState.Lose);
                return;
            } else {
                ChangeState(GameState.WaitingInput);
                return;
            }
        }

        ChangeState(_blocks.Any(b => b.Value == WinCondition) ? GameState.Win : GameState.WaitingInput);
    }

    private void SpawnBlock(Node node, int value) {
        var block = Instantiate(_blockPrefab, node.Position, Quaternion.identity);
        block.Init(GetBlockTypeByValue(value));
        block.SetBlock(node);
        _blocks.Add(block);
    }

    private bool CheckIfCanMove() {
        var orderedBlocks = _blocks.OrderBy(b => b.Position.x).ThenBy(b => b.Position.y).ToList();
        foreach (var block in orderedBlocks) {
            var next = block.Node;
            do {
                block.SetBlock(next);

                var possibleNodeRight = GetNodeAtPosition(next.Position + Vector2.right);
                var possibleNodeLeft = GetNodeAtPosition(next.Position + Vector2.left);
                var possibleNodeUp = GetNodeAtPosition(next.Position + Vector2.up);
                var possibleNodeDown = GetNodeAtPosition(next.Position + Vector2.down);

                if (possibleNodeRight?.OccupiedBlock.Value == block.Value) return true;
                if (possibleNodeLeft?.OccupiedBlock.Value == block.Value) return true;
                if (possibleNodeUp?.OccupiedBlock.Value == block.Value) return true;
                if (possibleNodeDown?.OccupiedBlock.Value == block.Value) return true;
            } while (next != block.Node);

        }
        return false;
    }

    private void Update() {
        if (_gameState != GameState.WaitingInput) {
            return;
        }

        if (_playerInput.GetInput() != Vector2.zero) {
            Shift(_playerInput.GetInput());
        }
    }

    private void Shift(Vector2 dir) {
        ChangeState(GameState.Moving);
        var orderedBlocks = _blocks.OrderBy(b => b.Position.x).ThenBy(b => b.Position.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

        foreach (var block in orderedBlocks) {
            var next = block.Node;
            do {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Position + dir);
                if (possibleNode != null) {
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Value)) {
                        block.MergeBlock(possibleNode.OccupiedBlock);
                    } else if (possibleNode.OccupiedBlock == null) next = possibleNode;
                }
            } while (next != block.Node);
        }

        var sequence = DOTween.Sequence();

        int notMoveCount = 0;

        foreach (var block in orderedBlocks) {
            var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Position : block.Node.Position;

            if (movePoint == block.Position) {
                notMoveCount++;
            }

            sequence.Insert(0, block.transform.DOMove(movePoint, _travelTime).SetEase(Ease.InQuad));
        }

        sequence.OnComplete(() => {
            var mergeBlocks = orderedBlocks.Where(b => b.MergingBlock != null).ToList();
            foreach (var block in mergeBlocks) {
                MergeBlocks(block.MergingBlock, block);
            }

            if (notMoveCount == orderedBlocks.Count) ChangeState(GameState.WaitingInput);
            else ChangeState(GameState.SpawningBlocks);

        });

    }

    private Node GetNodeAtPosition(Vector2 pos) {
        return _nodes.FirstOrDefault(n => n.Position == pos);
    }

    private void MergeBlocks(Block baseBlock, Block mergingBlock) {
        var newValue = baseBlock.Value * 2;
        _scoreManager.Score += _scoreMultiplier * newValue;

        RemoveBlock(baseBlock);

        SpawnBlock(baseBlock.Node, newValue);

        RemoveBlock(mergingBlock);
    }

    private void RemoveBlock(Block block) {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }
}


[Serializable]
public struct BlockType {
    public int Value;
    public Color Color;
    public Color TextColor;
    public float FontSize;
}

public enum GameState {
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose,
    Upgrade
}