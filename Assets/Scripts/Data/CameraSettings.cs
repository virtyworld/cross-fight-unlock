using UnityEngine;

namespace CrossFightUnlock.Data
{
    /// <summary>
    /// ScriptableObject для настроек камеры
    /// </summary>
    [CreateAssetMenu(fileName = "CameraSettings", menuName = "Cross Fight Unlock/Camera Settings")]
    public class CameraSettings : ScriptableObject
    {
        [Header("General Camera Settings")]
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private bool useBounds = false;
        [SerializeField] private Vector3 minBounds = new Vector3(-50, -50, -50);
        [SerializeField] private Vector3 maxBounds = new Vector3(50, 50, 50);

        [Header("Runner Mode")]
        [SerializeField] private bool isRunnerMode = true;
        [SerializeField] private float runnerHeight = 8f;
        [SerializeField] private float runnerDistance = 12f;
        [SerializeField] private float runnerForward = 2f;
        [SerializeField, Tooltip("Фиксированный поворот камеры в режиме раннера (X, Y, Z в градусах)")]
        private Vector3 runnerFixedRotation = new Vector3(25, 0, 0);

        #region Properties

        // General Settings
        public float SmoothSpeed => smoothSpeed;
        public bool UseBounds => useBounds;
        public Vector3 MinBounds => minBounds;
        public Vector3 MaxBounds => maxBounds;

        // Runner Mode
        public bool IsRunnerMode => isRunnerMode;
        public float RunnerHeight => runnerHeight;
        public float RunnerDistance => runnerDistance;
        public float RunnerForward => runnerForward;
        /// <summary>
        /// Фиксированный поворот камеры в режиме раннера
        /// </summary>
        public Vector3 RunnerFixedRotation => runnerFixedRotation;


        #endregion

        #region Public Methods

        /// <summary>
        /// Получение настроек для указанного режима
        /// </summary>
        public CameraModeSettings GetModeSettings(bool isRunner)
        {
            if (isRunner)
            {
                return new CameraModeSettings
                {
                    offset = new Vector3(runnerForward, runnerHeight, -runnerDistance),
                    fixedRotation = runnerFixedRotation,
                    lookAtTarget = false
                };
            }
            else
            {
                return new CameraModeSettings
                {
                    offset = new Vector3(runnerForward, runnerHeight, -runnerDistance),
                    fixedRotation = runnerFixedRotation,
                    lookAtTarget = false
                };
            }
        }

        /// <summary>
        /// Валидация настроек
        /// </summary>
        public void ValidateSettings()
        {
            smoothSpeed = Mathf.Max(0.1f, smoothSpeed);
            runnerHeight = Mathf.Max(1f, runnerHeight);
            runnerDistance = Mathf.Max(1f, runnerDistance);
            runnerForward = Mathf.Max(0f, runnerForward);
        }

        #endregion

        #region Editor Validation

        private void OnValidate()
        {
            ValidateSettings();
        }

        #endregion
    }

    /// <summary>
    /// Структура для настроек режима камеры
    /// </summary>
    [System.Serializable]
    public struct CameraModeSettings
    {
        public Vector3 offset;
        public bool lookAtTarget;
        public Vector3 fixedRotation;
    }
}
