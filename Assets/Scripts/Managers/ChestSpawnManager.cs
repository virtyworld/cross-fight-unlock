using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;
using CrossFightUnlock.Models;
using CrossFightUnlock.Presenters;
using UnityEngine.UI;

namespace CrossFightUnlock.Managers
{
    /// <summary>
    /// Менеджер для управления спавном сундуков
    /// </summary>
    public class ChestSpawnManager : MonoBehaviour, IManager
    {
        [Header("Chest Spawn Settings")]
        [SerializeField] private GameObject _chestPrefab;

        [Header("Debug")]
        [SerializeField] private bool _showDebugInfo = true;

        // Компоненты
        private GameEvents _gameEvents;
        private ChestModel _chestModel;

        // Состояние
        private bool _isInitialized = false;

        #region Unity Lifecycle       

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Инициализация менеджера
        /// </summary>
        public void Initialize(GameEvents gameEvents)
        {
            if (_isInitialized) return;

            _gameEvents = gameEvents;

            // Проверяем префаб сундука
            if (_chestPrefab == null)
            {
                Debug.LogError("ChestSpawnManager: Chest prefab not assigned!");
                return;
            }

            // Создаем модель сундука
            _chestModel = new ChestModel(_gameEvents, _chestPrefab);
            _chestModel.Initialize();

            _isInitialized = true;
            Debug.Log("ChestSpawnManager initialized");
        }

        /// <summary>
        /// Очистка менеджера
        /// </summary>
        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Очищаем модель сундука
            _chestModel?.Cleanup();
            _chestModel = null;

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
        /// Уничтожение сундука
        /// </summary>
        public void DestroyChest()
        {
            if (!_isInitialized) return;
            _chestModel?.DestroyChest();
        }

        /// <summary>
        /// Проверка, заспавнен ли сундук
        /// </summary>
        public bool IsChestSpawned()
        {
            return _isInitialized && _chestModel?.IsChestSpawned() == true;
        }

        /// <summary>
        /// Получение заспавненного сундука
        /// </summary>
        public ChestPresenter GetSpawnedChest()
        {
            return _chestModel?.GetSpawnedChest();
        }

        #endregion



    }
}
