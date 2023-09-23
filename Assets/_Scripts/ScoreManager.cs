using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private UIManager _uiManager;

    private float _score;

    public float Score { 
        get { return _score; }
        set {
            _score = value;
            var intScore = (int)_score;
            _uiManager.ScoreValue.text = intScore.ToString();
        }
    }

    public void Initializer(UIManager uIManager) {
           _uiManager = uIManager;
    }
}
