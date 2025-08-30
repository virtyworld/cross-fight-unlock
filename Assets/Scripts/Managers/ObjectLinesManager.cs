using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;
using CrossFightUnlock.Presenters;

namespace CrossFightUnlock.Managers
{
    /// <summary>
    /// Менеджер для управления линиями движения объектов
    /// </summary>
    public class ObjectLinesManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private ObjectLinesSettings _settings;

        [Header("Presenter")]
        [SerializeField] private ObjectLinesPresenter _presenter;

        // Состояние
        private bool _isInitialized = false;

        // Свойства
        public ObjectLinesSettings Settings => _settings;
        public IObjectLinesPresenter Presenter => _presenter;

        private void Awake()
        {
            // Автоматически находим презентер если он не назначен
            if (_presenter == null)
            {
                _presenter = GetComponent<ObjectLinesPresenter>();
                if (_presenter == null)
                {
                    _presenter = gameObject.AddComponent<ObjectLinesPresenter>();
                }
            }
        }


        /// <summary>
        /// Инициализация менеджера
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            if (_settings == null)
            {
                Debug.LogError("ObjectLinesSettings not assigned to ObjectLinesManager!");
                return;
            }

            if (_presenter == null)
            {
                Debug.LogError("ObjectLinesPresenter not found!");
                return;
            }

            // Инициализируем презентер
            _presenter.Initialize(_settings);

            _isInitialized = true;
            Debug.Log("ObjectLinesManager initialized");
        }

        /// <summary>
        /// Запуск всех линий
        /// </summary>
        public void StartAllLines()
        {
            Debug.Log($"ObjectLinesManager {_isInitialized}");
            if (!_isInitialized) return;
            _presenter.StartAllLines();
        }

        /// <summary>
        /// Остановка всех линий
        /// </summary>
        public void StopAllLines()
        {
            if (!_isInitialized) return;
            _presenter.StopAllLines();
        }

        /// <summary>
        /// Пауза всех линий
        /// </summary>
        public void PauseAllLines()
        {
            if (!_isInitialized) return;
            _presenter.PauseAllLines();
        }

        /// <summary>
        /// Возобновление всех линий
        /// </summary>
        public void ResumeAllLines()
        {
            if (!_isInitialized) return;
            _presenter.ResumeAllLines();
        }

        /// <summary>
        /// Управление конкретной линией
        /// </summary>
        public void StartLine(int lineIndex)
        {
            if (!_isInitialized) return;
            _presenter.StartLine(lineIndex);
        }

        public void StopLine(int lineIndex)
        {
            if (!_isInitialized) return;
            _presenter.StopLine(lineIndex);
        }

        public void PauseLine(int lineIndex)
        {
            if (!_isInitialized) return;
            _presenter.PauseLine(lineIndex);
        }

        public void ResumeLine(int lineIndex)
        {
            if (!_isInitialized) return;
            _presenter.ResumeLine(lineIndex);
        }

        /// <summary>
        /// Изменение направления движения для конкретной линии
        /// </summary>
        public void ChangeLineDirection(int lineIndex, bool moveTowardsPositiveX)
        {
            if (!_isInitialized) return;
            _presenter.ChangeLineDirection(lineIndex, moveTowardsPositiveX);
        }

        /// <summary>
        /// Изменение скорости движения для конкретной линии
        /// </summary>
        public void ChangeLineSpeed(int lineIndex, float newSpeed)
        {
            if (!_isInitialized) return;
            _presenter.ChangeLineSpeed(lineIndex, newSpeed);
        }

        /// <summary>
        /// Очистка всех объектов с конкретной линии
        /// </summary>
        public void ClearLine(int lineIndex)
        {
            if (!_isInitialized) return;
            _presenter.ClearLine(lineIndex);
        }

        /// <summary>
        /// Получение информации о линии
        /// </summary>
        public int GetLineObjectCount(int lineIndex)
        {
            if (!_isInitialized) return 0;
            return _presenter.GetLineObjectCount(lineIndex);
        }

        public bool IsLineActive(int lineIndex)
        {
            if (!_isInitialized) return false;
            return _presenter.IsLineActive(lineIndex);
        }

        public bool GetLineDirection(int lineIndex)
        {
            if (!_isInitialized) return true;
            return _presenter.GetLineDirection(lineIndex);
        }

        public float GetLineSpeed(int lineIndex)
        {
            if (!_isInitialized) return 0f;
            return _presenter.GetLineSpeed(lineIndex);
        }

        /// <summary>
        /// Получение общих настроек
        /// </summary>
        public ObjectLinesSettings GetSettings()
        {
            return _settings;
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        public void Cleanup()
        {
            if (!_isInitialized) return;

            if (_presenter != null)
            {
                _presenter.Cleanup();
            }

            _isInitialized = false;
            Debug.Log("ObjectLinesManager cleaned up");
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        // Методы для отладки и тестирования
        [ContextMenu("Start All Lines")]
        private void DebugStartAllLines()
        {
            StartAllLines();
        }

        [ContextMenu("Stop All Lines")]
        private void DebugStopAllLines()
        {
            StopAllLines();
        }

        [ContextMenu("Pause All Lines")]
        private void DebugPauseAllLines()
        {
            PauseAllLines();
        }

        [ContextMenu("Resume All Lines")]
        private void DebugResumeAllLines()
        {
            ResumeAllLines();
        }

        [ContextMenu("Change Line 0 Direction")]
        private void DebugChangeLine0Direction()
        {
            bool currentDirection = GetLineDirection(0);
            ChangeLineDirection(0, !currentDirection);
        }

        [ContextMenu("Change Line 1 Direction")]
        private void DebugChangeLine1Direction()
        {
            bool currentDirection = GetLineDirection(1);
            ChangeLineDirection(1, !currentDirection);
        }

        [ContextMenu("Change Line 2 Direction")]
        private void DebugChangeLine2Direction()
        {
            bool currentDirection = GetLineDirection(2);
            ChangeLineDirection(2, !currentDirection);
        }
    }
}
