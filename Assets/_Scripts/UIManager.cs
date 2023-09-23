using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour {
    [SerializeField] private GameObject _ui;
    private VisualElement _uiDocument;
    public Label TextGameState;
    public Button ResetButton;
    public Label ScoreValue;
    private VisualElement _debugVE;

    [SerializeField] private bool _debug;
    public bool Debug {
        get { return _debug; }
        set {
            _debug = value;
            if (_debugVE == null) return;
            if (_debug)
                _debugVE.style.visibility = Visibility.Visible;
            else
                _debugVE.style.visibility = Visibility.Hidden;
        }
    }

    private void OnValidate() {
        Debug = _debug;
    }

    void Awake() {
        _uiDocument = _ui.GetComponent<UIDocument>().rootVisualElement;

        ScoreValue = _uiDocument.Q<Label>("Value");
        TextGameState = _uiDocument.Q<Label>("GameState");
        ResetButton = _uiDocument.Q<Button>("ResetButton");
        _debugVE = _uiDocument.Q<VisualElement>("Debug");
    }
}
