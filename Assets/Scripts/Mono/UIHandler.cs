using CrossFightUnlock.Data;
using CrossFightUnlock.Models;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _chestButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private ChestModel _chestModel;
    [SerializeField] private GameObject winPanel;

    private GameEvents _gameEvents;

    public void Initialize(GameEvents gameEvents)
    {
        _gameEvents = gameEvents;
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _chestButton.onClick.AddListener(OnChestButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        _gameEvents.OnShowChestButtonUI.AddListener(OnShowChestButtonUI);
        _gameEvents.OnLockUnlocked.AddListener(OnLockUnlocked);
    }

    void Start()
    {
        _chestButton.gameObject.SetActive(false);
        winPanel.SetActive(false);
    }
    private void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked");
        _gameEvents.OnPlayerAttackInput?.Invoke();
    }

    private void OnChestButtonClicked()
    {
        Debug.Log("Chest button clicked");
        _gameEvents.OnChestOpened?.Invoke(_chestModel?.ChestGameObject);
    }

    private void OnShowChestButtonUI(bool show)
    {
        _chestButton.gameObject.SetActive(show);
    }

    private void OnRestartButtonClicked()
    {
        Debug.Log("Restart button clicked");
        _gameEvents.OnRestartButtonClicked?.Invoke();
    }

    private void OnLockUnlocked()
    {
        Debug.Log("Lock unlocked");
        winPanel.SetActive(true);
    }
}
