using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Views
{
    /// <summary>
    /// Представление триггера финиша, который активирует спавн врагов
    /// </summary>
    public class FinishTriggerView : MonoBehaviour, IView
    {
        [Header("Trigger Settings")]
        [SerializeField] private string triggerTag = "Player";
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.red;

        // События
        private GameEvents _gameEvents;

        // Состояние
        private bool _isInitialized = false;
        private bool _isVisible = true;
        private Collider _triggerCollider;

        public void Initialize()
        {
            if (_isInitialized) return;

            // Получаем или создаем коллайдер-триггер
            _triggerCollider = GetComponent<Collider>();
            if (_triggerCollider == null)
            {
                _triggerCollider = gameObject.AddComponent<BoxCollider>();
                _triggerCollider.isTrigger = true;
                Debug.Log("Added BoxCollider to FinishTriggerView");
            }
            else
            {
                _triggerCollider.isTrigger = true;
            }

            _isInitialized = true;
            Debug.Log("FinishTriggerView initialized");
        }

        public void Show()
        {
            if (!_isInitialized) return;

            _isVisible = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!_isInitialized) return;

            _isVisible = false;
            gameObject.SetActive(false);
        }

        public void Cleanup()
        {
            _gameEvents = null;
            _isInitialized = false;
        }

        /// <summary>
        /// Установка событий игры
        /// </summary>
        public void SetGameEvents(GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
        }

        /// <summary>
        /// Обработка входа в триггер
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"OnTriggerEnter {other.gameObject.name} _isInitialized {_isInitialized} _gameEvents {_gameEvents}");
            if (!_isInitialized || _gameEvents == null) return;

            // Проверяем, что вошел игрок
            if (other.CompareTag(triggerTag))
            {
                Debug.Log($"Player entered finish trigger: {gameObject.name}");
                _gameEvents.OnFinishTriggerEntered?.Invoke(other.gameObject);
            }
        }

        /// <summary>
        /// Обработка выхода из триггера
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            Debug.Log($"OnTriggerExit {other.gameObject.name} _isInitialized {_isInitialized} _gameEvents {_gameEvents}");
            if (!_isInitialized || _gameEvents == null) return;

            // Проверяем, что вышел игрок
            if (other.CompareTag(triggerTag))
            {
                Debug.Log($"Player exited finish trigger: {gameObject.name}");
                _gameEvents.OnFinishTriggerExited?.Invoke(other.gameObject);
            }
        }
    }
}
