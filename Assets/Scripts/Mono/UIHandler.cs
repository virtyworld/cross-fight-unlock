using CrossFightUnlock.Data;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Button _startButton;

    private GameEvents _gameEvents;

    public void Initialize(GameEvents gameEvents)
    {
        _gameEvents = gameEvents;
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked");
        _gameEvents.OnPlayerAttackInput?.Invoke();
    }
}
