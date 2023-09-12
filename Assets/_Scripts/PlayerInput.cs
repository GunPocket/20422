using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
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

    public Vector2 GetInput() {
        return PlayerController.Controls.MovePieces.ReadValue<Vector2>();
    }
}
