using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour {

    public int Value;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private TextMeshPro text;

    public void Init(BlockType type) {
        Value = type.Value;
        spriteRenderer.color = type.Color;
        text.text = type.Value.ToString();
        text.color = type.TextColor; 
        text.fontSize = type.FontSize;
    }
}
