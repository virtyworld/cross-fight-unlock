using UnityEngine;

namespace CrossFightUnlock.Data
{
    /// <summary>
    /// Настройки для всех трех линий движения объектов
    /// </summary>
    [CreateAssetMenu(fileName = "ObjectLinesSettings", menuName = "Cross Fight Unlock/Object Lines Settings")]
    public class ObjectLinesSettings : ScriptableObject
    {
        [Header("Line 1 Settings")]
        [SerializeField] private ObjectSpawnSettings _line1Settings;

        [Header("Line 2 Settings")]
        [SerializeField] private ObjectSpawnSettings _line2Settings;

        [Header("Line 3 Settings")]
        [SerializeField] private ObjectSpawnSettings _line3Settings;

        [Header("Global Settings")]
        [SerializeField] private bool _enableSpawning = true;
        [SerializeField] private float _globalSpawnMultiplier = 1f;
        [SerializeField] private bool _pauseBetweenWaves = false;
        [SerializeField] private float _wavePauseDuration = 5f;

        // Свойства для доступа к настройкам
        public ObjectSpawnSettings Line1Settings => _line1Settings;
        public ObjectSpawnSettings Line2Settings => _line2Settings;
        public ObjectSpawnSettings Line3Settings => _line3Settings;
        public bool EnableSpawning => _enableSpawning;
        public float GlobalSpawnMultiplier => _globalSpawnMultiplier;
        public bool PauseBetweenWaves => _pauseBetweenWaves;
        public float WavePauseDuration => _wavePauseDuration;

        /// <summary>
        /// Получить настройки для конкретной линии
        /// </summary>
        public ObjectSpawnSettings GetLineSettings(int lineIndex)
        {
            switch (lineIndex)
            {
                case 0: return _line1Settings;
                case 1: return _line2Settings;
                case 2: return _line3Settings;
                default: return null;
            }
        }

        /// <summary>
        /// Проверить, что все настройки линий настроены
        /// </summary>
        public bool AreAllLinesConfigured()
        {
            return _line1Settings != null && _line2Settings != null && _line3Settings != null;
        }

        /// <summary>
        /// Получить общее количество линий
        /// </summary>
        public int GetLinesCount()
        {
            return 3;
        }

        private void OnValidate()
        {
            _globalSpawnMultiplier = Mathf.Max(0.1f, _globalSpawnMultiplier);
            _wavePauseDuration = Mathf.Max(0f, _wavePauseDuration);
        }
    }
}
