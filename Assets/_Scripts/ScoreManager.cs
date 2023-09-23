using UnityEngine;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour {
    [SerializeField] private GameObject _ui;
    private VisualElement _uiDocument;
    [SerializeField] public Label ScoreValue;

    [SerializeField] private int _score;

    [SerializeField]
    public int Score {
        get {
            return _score;
        }
        set {
            if (value > 0)
                _score = value;
            else
                return;
        }
    }

    // Start is called before the first frame update
    void Awake() {
        _uiDocument = _ui.GetComponent<UIDocument>().rootVisualElement;

        ScoreValue = _uiDocument.Q<Label>("Value");
    }

    // Update is called once per frame
    void Update() {
        ScoreValue.text = _score.ToString();
    }
}