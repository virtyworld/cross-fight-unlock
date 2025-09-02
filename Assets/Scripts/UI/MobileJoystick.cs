using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.UI
{
    /// <summary>
    /// Мобильный джойстик для управления персонажем
    /// </summary>
    public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Joystick Settings")]
        [SerializeField] private RectTransform _joystickBackground;
        [SerializeField] private RectTransform _joystickHandle;
        [SerializeField] private float _joystickRange = 75f; // Радиус визуального джойстика
        [SerializeField] private float _inputRange = 75f; // Радиус для расчета ввода (равен визуальному)
        [SerializeField] private float _deadZone = 0.0f; // Убираем dead zone полностью
        [SerializeField] private float _sensitivity = 1.0f; // Прямое соответствие без множителей

        [Header("Visual Settings")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _pressedColor = Color.gray;
        [SerializeField] private float _smoothTime = 0.05f; // Уменьшили время плавности для резкости
        [SerializeField] private bool _instantReturn = true; // Мгновенное возвращение в центр для резкости

        // События
        private GameEvents _gameEvents;

        // Состояние джойстика
        private Vector2 _joystickInput = Vector2.zero;
        private Vector2 _joystickCenter;
        private bool _isPressed = false;
        private int _pointerId = -1;

        // Визуальные компоненты
        private Image _backgroundImage;
        private Image _handleImage;

        // Плавное возвращение
        private Vector2 _velocity = Vector2.zero;

        public void Initialize(GameEvents gameEvents)
        {
            _gameEvents = gameEvents;

            // Получаем компоненты изображений
            _backgroundImage = _joystickBackground.GetComponent<Image>();
            _handleImage = _joystickHandle.GetComponent<Image>();

            // Устанавливаем начальную позицию - центр всегда в нуле относительно контейнера
            _joystickCenter = Vector2.zero;
            _joystickHandle.anchoredPosition = _joystickCenter;

            // Устанавливаем начальный цвет
            if (_backgroundImage != null)
                _backgroundImage.color = _normalColor;
            if (_handleImage != null)
                _handleImage.color = _normalColor;

            Debug.Log($"MobileJoystick initialized - Center: {_joystickCenter}, Range: {_joystickRange}, Sensitivity: {_sensitivity}");
        }

        private void Update()
        {
            if (!_isPressed)
            {
                if (_instantReturn)
                {
                    // Мгновенное возвращение в центр для резкости
                    _joystickHandle.anchoredPosition = _joystickCenter;
                }
                else
                {
                    // Плавное возвращение в центр
                    _joystickHandle.anchoredPosition = Vector2.SmoothDamp(
                        _joystickHandle.anchoredPosition,
                        _joystickCenter,
                        ref _velocity,
                        _smoothTime
                    );
                }

                // Обнуляем ввод
                _joystickInput = Vector2.zero;
                SendMovementInput();
            }
            else
            {
                // Если джойстик нажат, продолжаем отправлять ввод каждый кадр
                // Это обеспечивает непрерывное движение персонажа
                SendMovementInput();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isPressed) return;

            _isPressed = true;
            _pointerId = eventData.pointerId;

            // Изменяем цвет при нажатии
            if (_backgroundImage != null)
                _backgroundImage.color = _pressedColor;
            if (_handleImage != null)
                _handleImage.color = _pressedColor;

            // Сразу обрабатываем касание для быстрого отклика
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId) return;

            _isPressed = false;
            _pointerId = -1;

            // Возвращаем цвет
            if (_backgroundImage != null)
                _backgroundImage.color = _normalColor;
            if (_handleImage != null)
                _handleImage.color = _normalColor;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isPressed || eventData.pointerId != _pointerId) return;

            // Получаем позицию касания относительно джойстика
            Vector2 touchPosition = eventData.position;
            Vector2 localPosition;

            // Конвертируем позицию экрана в локальную позицию относительно джойстика
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _joystickBackground,
                touchPosition,
                eventData.pressEventCamera,
                out localPosition
            );

            // Вычисляем расстояние от центра
            float distance = localPosition.magnitude;

            // Ручка джойстика следует за пальцем, но ограничена радиусом
            Vector2 handlePosition;
            if (distance > _joystickRange)
            {
                // Если палец за пределами радиуса, ручка остается на границе
                handlePosition = localPosition.normalized * _joystickRange;
            }
            else
            {
                // Если палец в пределах радиуса, ручка точно под пальцем
                handlePosition = localPosition;
            }

            // Устанавливаем позицию ручки
            _joystickHandle.anchoredPosition = handlePosition;

            // ПРЯМОЕ СООТВЕТСТВИЕ: ввод джойстика = позиция ручки относительно центра
            _joystickInput = handlePosition / _joystickRange;
        }

        /// <summary>
        /// Отправляет ввод движения в систему событий
        /// </summary>
        private void SendMovementInput()
        {
            if (_gameEvents == null) return;

            // Конвертируем 2D ввод в 3D (X и Z оси)
            Vector3 moveInput = new Vector3(_joystickInput.x, 0f, _joystickInput.y);

            // Отправляем событие движения
            _gameEvents.OnPlayerMoveInput?.Invoke(moveInput);


        }

        /// <summary>
        /// Получить текущий ввод джойстика
        /// </summary>
        public Vector2 GetJoystickInput()
        {
            return _joystickInput;
        }

        /// <summary>
        /// Проверить, активен ли джойстик
        /// </summary>
        public bool IsPressed()
        {
            return _isPressed;
        }

        /// <summary>
        /// Установить радиус джойстика (визуальный)
        /// </summary>
        public void SetJoystickRange(float range)
        {
            _joystickRange = Mathf.Max(10f, range);
        }

        /// <summary>
        /// Установить радиус ввода (для расчета движения)
        /// </summary>
        public void SetInputRange(float range)
        {
            _inputRange = Mathf.Max(_joystickRange, range);
        }

        /// <summary>
        /// Установить dead zone
        /// </summary>
        public void SetDeadZone(float deadZone)
        {
            _deadZone = Mathf.Clamp01(deadZone);
        }

        /// <summary>
        /// Установить чувствительность джойстика
        /// </summary>
        public void SetSensitivity(float sensitivity)
        {
            _sensitivity = Mathf.Max(0.1f, sensitivity);
            Debug.Log($"[MobileJoystick] Sensitivity set to: {_sensitivity}");
        }

        /// <summary>
        /// Получить текущую чувствительность
        /// </summary>
        public float GetSensitivity()
        {
            return _sensitivity;
        }

        /// <summary>
        /// Установить мгновенное возвращение в центр
        /// </summary>
        public void SetInstantReturn(bool instant)
        {
            _instantReturn = instant;
        }

        /// <summary>
        /// Установить время плавности возвращения
        /// </summary>
        public void SetSmoothTime(float smoothTime)
        {
            _smoothTime = Mathf.Max(0.01f, smoothTime);
        }

        /// <summary>
        /// Показать/скрыть джойстик
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// Настроить джойстик для прямого повторения движения пальца
        /// </summary>
        public void SetDirectMappingSettings()
        {
            _joystickRange = 75f;
            _inputRange = 75f;
            _deadZone = 0.0f;
            _sensitivity = 1.0f;
            _smoothTime = 0.0f;
            _instantReturn = true;

            Debug.Log("[MobileJoystick] Direct mapping settings applied - joystick follows finger exactly within 75px radius");
        }
    }
}
