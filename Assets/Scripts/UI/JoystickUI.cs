using UnityEngine;
using UnityEngine.UI;
using CrossFightUnlock.Data;

namespace CrossFightUnlock.UI
{
    /// <summary>
    /// UI компонент для создания мобильного джойстика
    /// </summary>
    public class JoystickUI : MonoBehaviour
    {
        [Header("Joystick Components")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _joystickContainer;
        [SerializeField] private RectTransform _joystickBackground;
        [SerializeField] private RectTransform _joystickHandle;
        [SerializeField] private MobileJoystick _mobileJoystick;

        [Header("Joystick Settings")]
        [SerializeField] private float _joystickSize = 150f; // Увеличили размер для удобства
        [SerializeField] private float _handleSize = 70f; // Увеличили размер ручки
        [SerializeField] private Color _backgroundColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color _handleColor = new Color(1f, 1f, 1f, 0.8f);



        public void Initialize(GameEvents gameEvents)
        {
            Debug.Log("JoystickUI Initialize");
            CreateJoystickUI();

            // Настраиваем джойстик
            _mobileJoystick.Initialize(gameEvents);

            Debug.Log("JoystickUI initialized");
        }

        private void CreateJoystickUI()
        {
            // Создаем Canvas если его нет
            if (_canvas == null)
            {
                GameObject canvasGO = new GameObject("JoystickCanvas");
                _canvas = canvasGO.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = 100; // Высокий приоритет отображения

                // Добавляем CanvasScaler для адаптивности
                CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;

                // Добавляем GraphicRaycaster для обработки касаний
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            // Создаем контейнер джойстика
            if (_joystickContainer == null)
            {
                GameObject containerGO = new GameObject("JoystickContainer");
                _joystickContainer = containerGO.AddComponent<RectTransform>();
                _joystickContainer.SetParent(_canvas.transform, false);

                // Позиционируем в левом нижнем углу
                _joystickContainer.anchorMin = new Vector2(0f, 0f);
                _joystickContainer.anchorMax = new Vector2(0f, 0f);
                _joystickContainer.anchoredPosition = new Vector2(180f, 180f); // Увеличили отступ
                _joystickContainer.sizeDelta = new Vector2(_joystickSize, _joystickSize);
            }

            // Создаем фон джойстика
            if (_joystickBackground == null)
            {
                GameObject backgroundGO = new GameObject("JoystickBackground");
                _joystickBackground = backgroundGO.AddComponent<RectTransform>();
                _joystickBackground.SetParent(_joystickContainer, false);

                // Настраиваем размер и позицию
                _joystickBackground.anchorMin = Vector2.zero;
                _joystickBackground.anchorMax = Vector2.one;
                _joystickBackground.offsetMin = Vector2.zero;
                _joystickBackground.offsetMax = Vector2.zero;

                // Добавляем Image компонент
                Image backgroundImage = backgroundGO.AddComponent<Image>();
                backgroundImage.color = _backgroundColor;
                backgroundImage.sprite = CreateCircleSprite();
                backgroundImage.type = Image.Type.Simple;
            }

            // Создаем ручку джойстика
            if (_joystickHandle == null)
            {
                GameObject handleGO = new GameObject("JoystickHandle");
                _joystickHandle = handleGO.AddComponent<RectTransform>();
                _joystickHandle.SetParent(_joystickContainer, false);

                // Настраиваем размер и позицию
                _joystickHandle.anchorMin = new Vector2(0.5f, 0.5f);
                _joystickHandle.anchorMax = new Vector2(0.5f, 0.5f);
                _joystickHandle.sizeDelta = new Vector2(_handleSize, _handleSize);
                _joystickHandle.anchoredPosition = Vector2.zero;

                // Добавляем Image компонент
                Image handleImage = handleGO.AddComponent<Image>();
                handleImage.color = _handleColor;
                handleImage.sprite = CreateCircleSprite();
                handleImage.type = Image.Type.Simple;
            }
        }

        /// <summary>
        /// Создает спрайт круга для джойстика
        /// </summary>
        private Sprite CreateCircleSprite()
        {
            int size = 64;
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            float radius = size * 0.5f - 1f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float distance = Vector2.Distance(pos, center);

                    if (distance <= radius)
                    {
                        pixels[y * size + x] = Color.white;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Получить компонент мобильного джойстика
        /// </summary>
        public MobileJoystick GetMobileJoystick()
        {
            return _mobileJoystick;
        }

        /// <summary>
        /// Показать/скрыть джойстик
        /// </summary>
        public void SetVisible(bool visible)
        {
            if (_joystickContainer != null)
            {
                _joystickContainer.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Установить позицию джойстика
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            if (_joystickContainer != null)
            {
                _joystickContainer.anchoredPosition = position;
            }
        }

        /// <summary>
        /// Установить размер джойстика
        /// </summary>
        public void SetSize(float size)
        {
            _joystickSize = size;

            if (_joystickContainer != null)
            {
                _joystickContainer.sizeDelta = new Vector2(_joystickSize, _joystickSize);
            }
        }
    }
}
