using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour { //https://www.youtube.com/watch?v=TeurfjuEIgA
    [SerializeField] private int width = 3;
    [SerializeField] private int height = 3;

    [SerializeField] private Block blockPrefab;
    [SerializeField] private Node nodePrefab;
    [SerializeField] private SpriteRenderer boardPrfab;

    [SerializeField] private List<BlockType> blockTypes;

    private List<Node> nodes;
    private List<Block> blocks;
    private GameState gameState;
    private int round;

    private BlockType GetBlockTypeByValue(int value) => blockTypes.First(t => t.Value == value);

    private void Start() {
        ChangeState(GameState.GenerateLevel);
    }

    private void ChangeState(GameState newState) {
        gameState = newState;

        switch (newState) {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void GenerateGrid() {
        round = 0;
        nodes = new List<Node>();
        blocks = new List<Block>();

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                var node = Instantiate(nodePrefab, new Vector2(i, j), Quaternion.identity);
                nodes.Add(node);
            }
        }

        var center = new Vector2((float)width / 2 - 0.5f, (float)height / 2 - 0.5f);

        var board = Instantiate(boardPrfab, center, Quaternion.identity);
        board.size = new Vector2(width, height);

        ChangeState(GameState.SpawningBlocks);
    }

    private void SpawnBlocks(int ammount) {

        var freeNodes = nodes.Where(x => x.OccupiedBlock == null).OrderBy(b => UnityEngine.Random.value).ToList();

        foreach (var node in freeNodes.Take(ammount)) {
            var block = Instantiate(blockPrefab, node.Position, Quaternion.identity);
            block.Init(GetBlockTypeByValue(UnityEngine.Random.value > 0.8f ? 4 : 2));
        }

        if (freeNodes.Count() == 1) {
            //F lost the game
            return;
        }
    }

    
}


[Serializable]
public struct BlockType {
    public int Value;
    public Color Color;
}

public enum GameState {
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}