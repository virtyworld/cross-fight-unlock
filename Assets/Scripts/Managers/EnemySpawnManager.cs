using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Presenters;

namespace CrossFightUnlock.Managers
{
    /// <summary>
    /// Менеджер-обертка для презентера спавна врагов
    /// Делегирует всю логику презентеру согласно MVP архитектуре
    /// </summary>
    public class EnemySpawnManager : MonoBehaviour, IManager
    {
        [Header("Presenter")]
        [SerializeField] private EnemySpawnPresenter _presenter;

        // Состояние
        private bool _isInitialized = false;

        private void Awake()
        {
            // Автоматически находим презентер если он не назначен
            if (_presenter == null)
            {
                _presenter = GetComponent<EnemySpawnPresenter>();
                if (_presenter == null)
                {
                    _presenter = gameObject.AddComponent<EnemySpawnPresenter>();
                }
            }
        }

        /// <summary>
        /// Инициализация менеджера
        /// </summary>
        public void Initialize(CrossFightUnlock.Data.GameEvents gameEvents)
        {
            if (_isInitialized) return;

            if (_presenter == null)
            {
                Debug.LogError("EnemySpawnPresenter not found!");
                return;
            }

            // Инициализируем презентер
            _presenter.Initialize(gameEvents);

            _isInitialized = true;
            Debug.Log("EnemySpawnManager initialized");
        }

        /// <summary>
        /// Очистка менеджера
        /// </summary>
        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Очищаем презентер
            _presenter?.Cleanup();

            _isInitialized = false;
        }

        /// <summary>
        /// Установка событий игры
        /// </summary>
        public void SetGameEvents(CrossFightUnlock.Data.GameEvents gameEvents)
        {
            _presenter?.SetGameEvents(gameEvents);
        }

        /// <summary>
        /// Очистка всех врагов
        /// </summary>
        public void ClearAllEnemies()
        {
            _presenter?.ClearAllEnemies();
        }

        /// <summary>
        /// Получение количества заспавненных врагов
        /// </summary>
        public int GetSpawnedEnemiesCount()
        {
            return _presenter?.GetSpawnedEnemiesCount() ?? 0;
        }

        /// <summary>
        /// Проверка, идет ли спавн
        /// </summary>
        public bool IsSpawning()
        {
            return _presenter?.IsSpawning() ?? false;
        }

        // Свойства для доступа к презентеру
        public IEnemySpawnPresenter Presenter => _presenter;
        public EnemySpawnPresenter PresenterComponent => _presenter;
    }
}
