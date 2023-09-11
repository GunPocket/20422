using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class tst : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D;

    public PlayerController PlayerController;

    private Vector3 movementDirection;

    public Vector3 MovementDirection {
        get {
            return movementDirection;
        }
        set {
            movementDirection = value;
        }
    }

    private void OnEnable() {
        PlayerController.Enable();
    }

    private void OnDisable() {
        PlayerController.Disable();
    }

    private void Awake() {
        RegisterEvents();
    }

    void RegisterEvents() {
        PlayerController = new PlayerController();

        var input = PlayerController.Controls;

        input.MovePieces.performed += ctx => { MovementDirection = input.MovePieces.ReadValue<Vector2>().normalized; };
    }
    public Vector2 GetMovementDirection() {
        return MovementDirection;
    }

    private void Start() {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        GetComponent<Rigidbody2D>().velocity = PlayerController.Controls.MovePieces.ReadValue<Vector2>() * 1.5f;
    }
}
