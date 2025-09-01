using UnityEngine;
using System.Collections.Generic;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Models
{
    /// <summary>
    /// Модель для управления спавном врагов
    /// </summary>
    public class EnemySpawnModel : IModel
    {
        // Настройки
        private EnemySpawnSettings _spawnSettings;
        private GameEvents _gameEvents;

        // Состояние
        private bool _isInitialized = false;
        private List<GameObject> _spawnedEnemies = new List<GameObject>();

        // Свойства       
        public int SpawnedEnemiesCount => _spawnedEnemies.Count;


        public EnemySpawnModel(EnemySpawnSettings spawnSettings, GameEvents gameEvents)
        {
            _spawnSettings = spawnSettings;
            _gameEvents = gameEvents;
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _spawnedEnemies.Clear();

            _isInitialized = true;
            Debug.Log("EnemySpawnModel initialized");
        }

        public void Cleanup()
        {
            _spawnSettings = null;
            _gameEvents = null;
            _spawnedEnemies.Clear();
            _isInitialized = false;
        }

        /// <summary>
        /// Активация триггера спавна
        /// </summary>
        public void TriggerSpawn()
        {
            if (!_isInitialized) return;

            _gameEvents.OnEnemySpawnTriggered?.Invoke();

            Debug.Log("Enemy spawn triggered");
        }


        /// <summary>
        /// Добавление врага в список заспавненных
        /// </summary>
        public void AddSpawnedEnemy(GameObject enemy)
        {
            if (enemy != null && !_spawnedEnemies.Contains(enemy))
            {
                _spawnedEnemies.Add(enemy);
                _gameEvents.OnEnemySpawned?.Invoke(enemy);
                Debug.Log($"Enemy spawned: {enemy.name}");
            }
        }

        /// <summary>
        /// Удаление врага из списка
        /// </summary>
        public void RemoveSpawnedEnemy(GameObject enemy)
        {
            if (_spawnedEnemies.Remove(enemy))
            {
                _gameEvents.OnEnemyDestroyed?.Invoke(enemy);
                Debug.Log($"Enemy removed: {enemy.name}");

                // Проверяем, уничтожены ли все враги
                CheckAllEnemiesDestroyed();
            }
        }

        /// <summary>
        /// Очистка всех заспавненных врагов
        /// </summary>
        public void ClearSpawnedEnemies()
        {
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy != null)
                {
                    _gameEvents.OnEnemyDestroyed?.Invoke(enemy);
                }
            }

            _spawnedEnemies.Clear();
            Debug.Log("All spawned enemies cleared");
        }

        /// <summary>
        /// Получение настроек спавна
        /// </summary>
        public EnemySpawnSettings GetSpawnSettings()
        {
            return _spawnSettings;
        }

        /// <summary>
        /// Проверка, можно ли спавнить врагов
        /// </summary>
        public bool CanSpawnEnemies()
        {
            return _isInitialized && _spawnedEnemies.Count < _spawnSettings.EnemiesToSpawn;
        }

        /// <summary>
        /// Получение количества врагов для спавна
        /// </summary>


        /// <summary>
        /// Проверка, уничтожены ли все враги
        /// </summary>
        private void CheckAllEnemiesDestroyed()
        {
            Debug.Log($"Checking all enemies destroyed:  {_spawnedEnemies.Count}");
            // Проверяем, что все враги заспавнены и все уничтожены
            if (_spawnedEnemies.Count == 0)
            {
                _gameEvents.OnAllEnemiesDestroyed?.Invoke();
                Debug.Log("All enemies destroyed!");
            }
        }
    }
}
