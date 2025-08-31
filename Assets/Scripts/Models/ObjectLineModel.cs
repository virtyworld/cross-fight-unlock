using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Models
{
    /// <summary>
    /// Модель линии движения объектов
    /// </summary>
    public class ObjectLineModel : MonoBehaviour, IObjectLineModel
    {
        [Header("References")]
        [SerializeField] private Transform _spawnParent;

        // Настройки
        private ObjectSpawnSettings _settings;

        // Состояние
        private bool _isInitialized = false;
        private bool _isSpawningActive = false;
        private bool _isPaused = false;
        private bool _currentDirection = true;
        private float _currentSpeed = 2f;

        // Спавн
        private Coroutine _spawnCoroutine;
        private List<GameObject> _activeObjects = new List<GameObject>();
        private float _nextSpawnTime = 0f;

        // Пул объектов
        private Queue<GameObject> _objectPool = new Queue<GameObject>();
        private int _maxPoolSize = 50;

        public void Initialize(ObjectSpawnSettings settings)
        {
            if (_isInitialized) return;

            _settings = settings;
            _currentDirection = settings.IsRightDirection;
            _currentSpeed = settings.MovementSpeed;

            // Создаем родительский объект для спавна если его нет
            if (_spawnParent == null)
            {
                GameObject spawnParentObj = new GameObject($"ObjectLine_{gameObject.name}");
                _spawnParent = spawnParentObj.transform;
                _spawnParent.SetParent(transform);
            }

            _isInitialized = true;
            Debug.Log($"ObjectLineModel initialized with settings: {settings.name}");
        }

        public void StartSpawning()
        {
            if (!_isInitialized || _isSpawningActive) return;

            _isSpawningActive = true;
            _isPaused = false;

            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }

            _spawnCoroutine = StartCoroutine(SpawnCoroutine());
            Debug.Log("Object spawning started");
        }

        public void StopSpawning()
        {
            if (!_isInitialized) return;

            _isSpawningActive = false;
            _isPaused = false;

            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }

            Debug.Log("Object spawning stopped");
        }

        public void PauseSpawning()
        {
            if (!_isInitialized) return;

            _isPaused = true;
            Debug.Log("Object spawning paused");
        }

        public void ResumeSpawning()
        {
            if (!_isInitialized) return;

            _isPaused = false;
            Debug.Log("Object spawning resumed");
        }

        public void ChangeDirection(bool moveTowardsPositiveX)
        {
            if (!_isInitialized) return;

            _currentDirection = moveTowardsPositiveX;

            // Обновляем направление всех активных объектов
            foreach (var obj in _activeObjects)
            {
                if (obj != null)
                {
                    var objectView = obj.GetComponent<IObjectView>();
                    if (objectView != null)
                    {
                        Vector3 direction = moveTowardsPositiveX ? Vector3.right : Vector3.left;
                        objectView.SetMovementDirection(direction);
                    }
                }
            }

            Debug.Log($"Direction changed to: {(moveTowardsPositiveX ? "Positive X" : "Negative X")}");
        }

        public void ChangeSpeed(float newSpeed)
        {
            if (!_isInitialized) return;

            _currentSpeed = newSpeed;

            // Обновляем скорость всех активных объектов
            foreach (var obj in _activeObjects)
            {
                if (obj != null)
                {
                    var objectView = obj.GetComponent<IObjectView>();
                    if (objectView != null)
                    {
                        objectView.SetMovementSpeed(newSpeed);
                    }
                }
            }

            Debug.Log($"Speed changed to: {newSpeed}");
        }

        public ObjectSpawnSettings GetSettings()
        {
            return _settings;
        }

        public int GetActiveObjectCount()
        {
            return _activeObjects.Count;
        }

        public List<GameObject> GetActiveObjects()
        {
            return new List<GameObject>(_activeObjects);
        }

        public void ClearAllObjects()
        {
            if (!_isInitialized) return;

            foreach (var obj in _activeObjects)
            {
                if (obj != null)
                {
                    ReturnToPool(obj);
                }
            }

            _activeObjects.Clear();
            Debug.Log("All objects cleared from line");
        }

        public bool IsSpawningActive()
        {
            return _isSpawningActive && !_isPaused;
        }

        public bool GetCurrentDirection()
        {
            return _currentDirection;
        }

        public float GetCurrentSpeed()
        {
            return _currentSpeed;
        }

        private IEnumerator SpawnCoroutine()
        {
            Debug.Log($"Test {_isSpawningActive} _isPaused {_isPaused}");
            while (_isSpawningActive)
            {
                if (!_isPaused)
                {
                    // Проверяем, можно ли спавнить
                    if (CanSpawnObjects())
                    {
                        SpawnObjects();

                        // Устанавливаем следующий интервал спавна
                        float spawnInterval = _settings.GetRandomSpawnInterval();
                        yield return new WaitForSeconds(spawnInterval);
                    }
                    else
                    {
                        // Ждем немного и проверяем снова
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        private bool CanSpawnObjects()
        {
            // Проверяем лимит объектов на линии
            if (_activeObjects.Count >= _settings.MaxObjectsOnLine)
                return false;

            // Проверяем минимальное расстояние между объектами
            if (_activeObjects.Count > 0)
            {
                Vector3 spawnPosition = _settings.GetRandomSpawnPosition();
                foreach (var obj in _activeObjects)
                {
                    if (obj != null)
                    {
                        float distance = Vector3.Distance(spawnPosition, obj.transform.position);
                        if (distance < _settings.MinDistanceBetweenObjects)
                            return false;
                    }
                }
            }

            return true;
        }

        private void SpawnObjects()
        {
            if (_settings.ObjectPrefab == null) return;

            int objectCount = _settings.GetRandomObjectCount();

            for (int i = 0; i < objectCount; i++)
            {
                if (_activeObjects.Count >= _settings.MaxObjectsOnLine) break;

                GameObject obj = GetFromPool();
                if (obj != null)
                {
                    SetupObject(obj);
                    _activeObjects.Add(obj);
                }
            }
        }

        private GameObject GetFromPool()
        {
            GameObject obj;

            if (_objectPool.Count > 0)
            {
                obj = _objectPool.Dequeue();
                obj.SetActive(true);

                // Переинициализируем PoolReturnComponent для повторно используемого объекта
                var poolReturn = obj.GetComponent<PoolReturnComponent>();
                if (poolReturn != null)
                {
                    poolReturn.Initialize(this, obj);
                }
            }
            else
            {
                obj = Instantiate(_settings.ObjectPrefab, _spawnParent);
                obj.transform.position = _settings.LineStartPosition;
                obj.name = "Object" + Random.Range(1, 1000000);
            }

            return obj;
        }

        private void SetupObject(GameObject obj)
        {
            // Устанавливаем позицию
            Vector3 spawnPosition = _settings.GetRandomSpawnPosition();
            obj.transform.position = spawnPosition;

            // Инициализируем ObjectView
            var objectView = obj.GetComponent<IObjectView>();
            if (objectView != null)
            {
                objectView.Initialize();
                objectView.Show(); // Показываем объект (важно для объектов из пула)
                objectView.SetPosition(spawnPosition);

                Vector3 direction = _currentDirection ? Vector3.right : Vector3.left;
                objectView.Move(direction, _currentSpeed);
            }

            // Добавляем компонент для автоматического возврата в пул
            var poolReturn = obj.GetComponent<PoolReturnComponent>();
            if (poolReturn == null)
            {
                poolReturn = obj.AddComponent<PoolReturnComponent>();
            }
            poolReturn.Initialize(this, obj);
        }

        public void ReturnToPool(GameObject obj)
        {
            if (obj == null) return;

            // Останавливаем движение
            var objectView = obj.GetComponent<IObjectView>();
            if (objectView != null)
            {
                objectView.Stop();
                objectView.Hide();
            }

            // Сбрасываем позицию объекта
            obj.transform.position = _settings.LineStartPosition;

            // Отключаем объект перед возвратом в пул
            obj.SetActive(false);

            // Возвращаем в пул
            if (_objectPool.Count < _maxPoolSize)
            {
                _objectPool.Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        private void OnDestroy()
        {
            StopSpawning();
            ClearAllObjects();
        }

        // Вспомогательный компонент для автоматического возврата в пул
        private class PoolReturnComponent : MonoBehaviour
        {
            private ObjectLineModel _lineModel;
            private GameObject _object;

            public void Initialize(ObjectLineModel lineModel, GameObject obj)
            {
                _lineModel = lineModel;
                _object = obj;
            }

            private void OnTriggerEnter(Collider other)
            {
                // Проверяем, если объект пересекает коллайдер с тегом "EndOfTheRoad"
                if (other.CompareTag("EndOfTheRoad"))
                {
                    // Деактивируем объект
                    if (_lineModel != null && _object != null)
                    {
                        _lineModel.RemoveFromActiveList(_object);
                        _lineModel.ReturnToPool(_object);
                    }
                }
            }
        }

        /// <summary>
        /// Удаление объекта из активного списка (для внутреннего использования)
        /// </summary>
        public void RemoveFromActiveList(GameObject obj)
        {
            _activeObjects.Remove(obj);
        }
    }
}
