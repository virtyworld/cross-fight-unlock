using UnityEngine;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.Views
{
    /// <summary>
    /// Компонент для следования камеры за игроком
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        private Transform target;
        private CameraModeSettings _currentSettings;
        private bool _isRunnerMode = true;
        private Camera _camera;
        private bool _isInitialized = false;
        private CameraSettings _cameraSettings;

        #region Unity Lifecycle

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                Debug.LogError("CameraFollow: Camera component not found!");
            }
        }

        private void FixedUpdate()
        {
            if (!_isInitialized || target == null) return;

            FollowTarget();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Инициализация компонента
        /// </summary>
        public void Initialize(Transform playerTransform, CameraSettings cameraSettings)
        {
            if (playerTransform == null)
            {
                Debug.LogError("CameraFollow: Player transform is null!");
                return;
            }

            target = playerTransform;
            _cameraSettings = cameraSettings;
            _isInitialized = true;

            Debug.Log($"CameraFollow: Initialized with target {target.name}");
        }

        /// <summary>
        /// Установка цели для следования
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            _isInitialized = target != null;
        }

        /// <summary>
        /// Очистка компонента
        /// </summary>
        public void Cleanup()
        {
            target = null;
            _isInitialized = false;
        }

        /// <summary>
        /// Получение текущей цели
        /// </summary>
        public Transform GetTarget()
        {
            return target;
        }

        /// <summary>
        /// Настройка камеры для режима раннера
        /// </summary>
        public void SetupRunnerCamera(float height, float distance, float forward)
        {
            if (_cameraSettings != null)
            {
                _isRunnerMode = true;
                _currentSettings = _cameraSettings.GetModeSettings(true);
                Debug.Log($"CameraFollow: Runner camera setup - Height: {height}, Distance: {distance}, Forward: {forward}");
            }
        }

        /// <summary>
        /// Переключение в обычный режим следования
        /// </summary>
        public void SetupNormalCamera()
        {
            if (_cameraSettings != null)
            {
                _isRunnerMode = false;
                _currentSettings = _cameraSettings.GetModeSettings(false);
                Debug.Log("CameraFollow: Switched to normal camera mode");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Следование за целью
        /// </summary>
        private void FollowTarget()
        {
            if (target == null || _cameraSettings == null) return;

            // Вычисляем желаемую позицию с учетом направления движения игрока
            Vector3 desiredPosition = CalculateDesiredPosition();

            // Применяем границы если включены
            if (_cameraSettings.UseBounds)
            {
                desiredPosition = ClampPosition(desiredPosition);
            }

            // Плавно перемещаем камеру
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _cameraSettings.SmoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // Устанавливаем поворот камеры
            if (_isRunnerMode)
            {
                // В режиме раннера используем фиксированный поворот
                if (_cameraSettings.RunnerFixedRotation != Vector3.zero)
                {
                    transform.rotation = Quaternion.Euler(_cameraSettings.RunnerFixedRotation);
                }
            }
            else
            {
                // В обычном режиме используем настройки из _currentSettings
                if (!_currentSettings.lookAtTarget)
                {
                    transform.rotation = Quaternion.Euler(_currentSettings.fixedRotation);
                }
                else
                {
                    // Если включен LookAt, используем его
                    transform.LookAt(target);
                }
            }
        }

        /// <summary>
        /// Вычисление желаемой позиции камеры с учетом направления движения
        /// </summary>
        private Vector3 CalculateDesiredPosition()
        {
            if (_isRunnerMode)
            {
                // Для раннера: камера располагается сверху и сзади игрока
                // Смещение вперед для лучшего обзора дороги
                Vector3 runnerPosition = target.position +
                    Vector3.forward * _cameraSettings.RunnerForward +
                    Vector3.up * _cameraSettings.RunnerHeight +
                    Vector3.back * _cameraSettings.RunnerDistance;
                return runnerPosition;
            }
            else
            {
                // Обычный режим следования
                return target.position + _currentSettings.offset;
            }
        }

        /// <summary>
        /// Ограничение позиции в заданных границах
        /// </summary>
        private Vector3 ClampPosition(Vector3 position)
        {
            return new Vector3(
                Mathf.Clamp(position.x, _cameraSettings.MinBounds.x, _cameraSettings.MaxBounds.x),
                Mathf.Clamp(position.y, _cameraSettings.MinBounds.y, _cameraSettings.MaxBounds.y),
                Mathf.Clamp(position.z, _cameraSettings.MinBounds.z, _cameraSettings.MaxBounds.z)
            );
        }

        #endregion

        #region Debug & Gizmos

        private void OnDrawGizmosSelected()
        {
            if (target == null || _cameraSettings == null) return;

            // Рисуем линию от камеры к цели
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);

            // Рисуем границы если включены
            if (_cameraSettings.UseBounds)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube((_cameraSettings.MinBounds + _cameraSettings.MaxBounds) * 0.5f,
                    _cameraSettings.MaxBounds - _cameraSettings.MinBounds);
            }
        }

        #endregion
    }
}
