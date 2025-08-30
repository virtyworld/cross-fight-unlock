using UnityEngine;
using CrossFightUnlock.Mono;

namespace CrossFightUnlock.Utils
{
    /// <summary>
    /// Простой контроллер паузы для тестирования
    /// </summary>
    public class PauseController : MonoBehaviour
    {
        [Header("Pause Settings")]
        [SerializeField] private KeyCode _pauseKey = KeyCode.P;
        [SerializeField] private bool _showPauseMenu = true;

        private GameHandler _gameHandler;
        private bool _isInitialized = false;

        private void Start()
        {
            // Ищем GameHandler в сцене
            _gameHandler = FindObjectOfType<GameHandler>();
            if (_gameHandler == null)
            {
                Debug.LogWarning("GameHandler not found! Pause functionality will not work.");
                return;
            }

            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized || _gameHandler == null) return;

            // Обработка нажатия клавиши паузы
            if (Input.GetKeyDown(_pauseKey))
            {
                TogglePause();
            }
        }

        /// <summary>
        /// Переключение паузы
        /// </summary>
        public void TogglePause()
        {
            if (_gameHandler == null) return;

            _gameHandler.TogglePause();

            if (_showPauseMenu)
            {
                ShowPauseMessage();
            }
        }

        /// <summary>
        /// Показать сообщение о паузе
        /// </summary>
        private void ShowPauseMessage()
        {
            if (_gameHandler.IsGamePaused())
            {
                Debug.Log("=== GAME PAUSED ===");
                Debug.Log("Press P to resume");
            }
            else
            {
                Debug.Log("=== GAME RESUMED ===");
            }
        }

        /// <summary>
        /// Принудительная пауза
        /// </summary>
        public void ForcePause()
        {
            if (_gameHandler == null) return;

            _gameHandler.PauseGame();
        }

        /// <summary>
        /// Принудительное возобновление
        /// </summary>
        public void ForceResume()
        {
            if (_gameHandler == null) return;

            _gameHandler.ResumeGame();
        }

        /// <summary>
        /// Получение состояния паузы
        /// </summary>
        public bool IsPaused()
        {
            return _gameHandler != null && _gameHandler.IsGamePaused();
        }
    }
}
