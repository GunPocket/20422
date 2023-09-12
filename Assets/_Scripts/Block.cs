using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour {

    public int Value;
    public Node Node;
    public Block MergingBlock;
    public bool Merging;

    public Vector2 Position => transform.position;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private TextMeshPro text;

    public void Init(BlockType type) {
        Value = type.Value;
        spriteRenderer.color = type.Color;
        text.text = type.Value.ToString();
        text.color = type.TextColor;
        text.fontSize = type.FontSize;
    }

    public void SetBlock(Node node) {
        if (Node != null) {
            Node.OccupiedBlock = null;
        }
        Node = node;
        Node.OccupiedBlock = this;
    }

    public void MergeBlock(Block blockToMergeWith) {
        MergingBlock = blockToMergeWith;

        Node.OccupiedBlock = null;

        blockToMergeWith.Merging = true;
    }

    public bool CanMerge(int value) => value == Value && !Merging && MergingBlock == null ;
}