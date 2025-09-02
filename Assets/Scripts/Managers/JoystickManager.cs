using UnityEngine;
using CrossFightUnlock.Data;
using CrossFightUnlock.UI;

namespace CrossFightUnlock.Managers
{
    /// <summary>
    /// Менеджер для управления мобильным джойстиком
    /// </summary>
    public class JoystickManager : MonoBehaviour
    {
        [Header("Joystick Settings")]
        [SerializeField] private JoystickUI _joystickUI;
        [SerializeField] private bool _autoDetectMobile = true;
        [SerializeField] private bool _showJoystickInEditor = false;

        // События игры
        private GameEvents _gameEvents;

        // Состояние
        private bool _isInitialized = false;
        private bool _isMobilePlatform = false;

        public void Initialize(GameEvents gameEvents)
        {
            Debug.Log("JoystickManager Initialize");
            if (_isInitialized) return;

            _gameEvents = gameEvents;

            // Определяем платформу
            _isMobilePlatform = Application.isMobilePlatform;
            Debug.Log("isMobilePlatform: " + _isMobilePlatform);
            Debug.Log("_joystickUI: " + _joystickUI);
            // Создаем UI джойстика если его нет
            if (_joystickUI == null)
            {
                CreateJoystickUI();
            }

            // Инициализируем джойстик
            if (_joystickUI != null)
            {
                _joystickUI.Initialize(_gameEvents);

                // Показываем джойстик на мобильных устройствах или в редакторе (если включено)
                bool shouldShow = _isMobilePlatform || (_showJoystickInEditor && Application.isEditor);
                _joystickUI.SetVisible(shouldShow);
            }

            // Подписываемся на события
            if (_gameEvents != null)
            {
                _gameEvents.OnCleanup.AddListener(HandleCleanup);
            }

            _isInitialized = true;

            Debug.Log($"JoystickManager initialized - Mobile: {_isMobilePlatform}");
        }

        private void CreateJoystickUI()
        {
            // Создаем объект с JoystickUI компонентом
            GameObject joystickGO = new GameObject("JoystickUI");
            _joystickUI = joystickGO.AddComponent<JoystickUI>();
        }

        /// <summary>
        /// Переключить видимость джойстика
        /// </summary>
        public void SetJoystickVisible(bool visible)
        {
            if (_joystickUI != null)
            {
                _joystickUI.SetVisible(visible);
            }
        }

        /// <summary>
        /// Установить позицию джойстика
        /// </summary>
        public void SetJoystickPosition(Vector2 position)
        {
            if (_joystickUI != null)
            {
                _joystickUI.SetPosition(position);
            }
        }

        /// <summary>
        /// Установить размер джойстика
        /// </summary>
        public void SetJoystickSize(float size)
        {
            if (_joystickUI != null)
            {
                _joystickUI.SetSize(size);
            }
        }

        /// <summary>
        /// Установить чувствительность джойстика
        /// </summary>
        public void SetJoystickSensitivity(float sensitivity)
        {
            MobileJoystick joystick = GetMobileJoystick();
            if (joystick != null)
            {
                joystick.SetSensitivity(sensitivity);
            }
        }

        /// <summary>
        /// Установить dead zone джойстика
        /// </summary>
        public void SetJoystickDeadZone(float deadZone)
        {
            MobileJoystick joystick = GetMobileJoystick();
            if (joystick != null)
            {
                joystick.SetDeadZone(deadZone);
            }
        }

        /// <summary>
        /// Установить радиус ввода джойстика
        /// </summary>
        public void SetJoystickInputRange(float inputRange)
        {
            MobileJoystick joystick = GetMobileJoystick();
            if (joystick != null)
            {
                joystick.SetInputRange(inputRange);
            }
        }

        /// <summary>
        /// Установить резкость джойстика
        /// </summary>
        public void SetJoystickSharpness(bool instantReturn, float smoothTime = 0.05f)
        {
            MobileJoystick joystick = GetMobileJoystick();
            if (joystick != null)
            {
                joystick.SetInstantReturn(instantReturn);
                joystick.SetSmoothTime(smoothTime);
            }
        }

        /// <summary>
        /// Применить настройки прямого повторения движения пальца
        /// </summary>
        public void ApplyDirectMappingSettings()
        {
            MobileJoystick joystick = GetMobileJoystick();
            if (joystick != null)
            {
                joystick.SetDirectMappingSettings();
                Debug.Log("[JoystickManager] Direct mapping settings applied");
            }
        }

        /// <summary>
        /// Получить компонент мобильного джойстика
        /// </summary>
        public MobileJoystick GetMobileJoystick()
        {
            if (_joystickUI != null)
            {
                return _joystickUI.GetMobileJoystick();
            }
            return null;
        }

        /// <summary>
        /// Проверить, является ли устройство мобильным
        /// </summary>
        public bool IsMobilePlatform()
        {
            return _isMobilePlatform;
        }

        /// <summary>
        /// Обработка события очистки
        /// </summary>
        private void HandleCleanup()
        {
            if (!_isInitialized) return;

            Debug.Log("JoystickManager: Cleanup event received");
            Cleanup();
        }

        public void Cleanup()
        {
            if (!_isInitialized) return;

            // Отписываемся от событий
            if (_gameEvents != null)
            {
                _gameEvents.OnCleanup.RemoveListener(HandleCleanup);
            }

            _gameEvents = null;
            _isInitialized = false;
        }
    }
}
