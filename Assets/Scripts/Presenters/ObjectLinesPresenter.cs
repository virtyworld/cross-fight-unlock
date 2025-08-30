using System.Collections.Generic;
using UnityEngine;
using CrossFightUnlock.Interfaces;
using CrossFightUnlock.Data;
using CrossFightUnlock.Models;

namespace CrossFightUnlock.Presenters
{
    /// <summary>
    /// Презентер для управления линиями движения объектов
    /// </summary>
    public class ObjectLinesPresenter : MonoBehaviour, IObjectLinesPresenter
    {
        [Header("Line Models")]
        [SerializeField] private List<ObjectLineModel> _lineModels = new List<ObjectLineModel>();

        // Настройки
        private ObjectLinesSettings _settings;

        // Состояние
        private bool _isInitialized = false;
        private Dictionary<int, IObjectLineModel> _lineModelsDict = new Dictionary<int, IObjectLineModel>();

        public void Initialize(ObjectLinesSettings settings)
        {
            if (_isInitialized) return;

            _settings = settings;

            if (!_settings.AreAllLinesConfigured())
            {
                Debug.LogError("Not all line settings are configured!");
                return;
            }

            // Инициализируем модели линий
            InitializeLineModels();

            _isInitialized = true;
            Debug.Log("ObjectLinesPresenter initialized");
        }

        private void InitializeLineModels()
        {
            _lineModelsDict.Clear();

            for (int i = 0; i < _settings.GetLinesCount(); i++)
            {
                if (i < _lineModels.Count && _lineModels[i] != null)
                {
                    var lineSettings = _settings.GetLineSettings(i);
                    if (lineSettings != null)
                    {
                        _lineModels[i].Initialize(lineSettings);
                        _lineModelsDict[i] = _lineModels[i];
                        Debug.Log($"Line {i} initialized with settings: {lineSettings.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Line {i} model is missing or null!");
                }
            }
        }

        public void StartAllLines()
        {
            Debug.Log($"ObjectLinePresenter {_isInitialized}");
            if (!_isInitialized) return;

            foreach (var lineModel in _lineModelsDict.Values)
            {
                lineModel.StartSpawning();
            }

            Debug.Log("All lines started");
        }

        public void StopAllLines()
        {
            if (!_isInitialized) return;

            foreach (var lineModel in _lineModelsDict.Values)
            {
                lineModel.StopSpawning();
            }

            Debug.Log("All lines stopped");
        }

        public void PauseAllLines()
        {
            if (!_isInitialized) return;

            foreach (var lineModel in _lineModelsDict.Values)
            {
                lineModel.PauseSpawning();
            }

            Debug.Log("All lines paused");
        }

        public void ResumeAllLines()
        {
            if (!_isInitialized) return;

            foreach (var lineModel in _lineModelsDict.Values)
            {
                lineModel.ResumeSpawning();
            }

            Debug.Log("All lines resumed");
        }

        public void StartLine(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return;

            _lineModelsDict[lineIndex].StartSpawning();
            Debug.Log($"Line {lineIndex} started");
        }

        public void StopLine(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return;

            _lineModelsDict[lineIndex].StopSpawning();
            Debug.Log($"Line {lineIndex} stopped");
        }

        public void PauseLine(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return;

            _lineModelsDict[lineIndex].PauseSpawning();
            Debug.Log($"Line {lineIndex} paused");
        }

        public void ResumeLine(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return;

            _lineModelsDict[lineIndex].ResumeSpawning();
            Debug.Log($"Line {lineIndex} resumed");
        }

        public void ChangeLineDirection(int lineIndex, bool moveTowardsPositiveX)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return;

            _lineModelsDict[lineIndex].ChangeDirection(moveTowardsPositiveX);
            Debug.Log($"Line {lineIndex} direction changed to: {(moveTowardsPositiveX ? "Positive X" : "Negative X")}");
        }

        public void ChangeLineSpeed(int lineIndex, float newSpeed)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return;

            _lineModelsDict[lineIndex].ChangeSpeed(newSpeed);
            Debug.Log($"Line {lineIndex} speed changed to: {newSpeed}");
        }

        public void ClearLine(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return;

            _lineModelsDict[lineIndex].ClearAllObjects();
            Debug.Log($"Line {lineIndex} cleared");
        }

        public int GetLineObjectCount(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return 0;

            return _lineModelsDict[lineIndex].GetActiveObjectCount();
        }

        public bool IsLineActive(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return false;

            return _lineModelsDict[lineIndex].IsSpawningActive();
        }

        public bool GetLineDirection(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return true;

            return _lineModelsDict[lineIndex].GetCurrentDirection();
        }

        public float GetLineSpeed(int lineIndex)
        {
            if (!_isInitialized || !_lineModelsDict.ContainsKey(lineIndex)) return 0f;

            return _lineModelsDict[lineIndex].GetCurrentSpeed();
        }

        public ObjectLinesSettings GetSettings()
        {
            return _settings;
        }

        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Останавливаем все линии
            StopAllLines();

            // Очищаем все линии
            foreach (var lineModel in _lineModelsDict.Values)
            {
                lineModel.ClearAllObjects();
            }

            _lineModelsDict.Clear();
            _settings = null;
            _isInitialized = false;

            Debug.Log("ObjectLinesPresenter cleaned up");
        }

        private void OnDestroy()
        {
            Cleanup();
        }
    }
}
