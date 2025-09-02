using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace CrossFightUnlock.Views
{
    public class KeyDragComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private KeyColor _keyColor;
        private CrossFightUnlock.Presenters.UnlockPresenter _unlockPresenter;
        private Vector3 _originalPosition;
        private Transform _originalParent;
        private CanvasGroup _canvasGroup;
        private bool _isDragging = false;

        public KeyColor KeyColor => _keyColor;

        public void Initialize(KeyColor keyColor, CrossFightUnlock.Presenters.UnlockPresenter unlockPresenter)
        {
            _keyColor = keyColor;
            _unlockPresenter = unlockPresenter;

            // Добавляем CanvasGroup для управления прозрачностью
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_unlockPresenter == null) return;

            _isDragging = true;
            _originalPosition = transform.position;
            _originalParent = transform.parent;

            // Делаем объект полупрозрачным и неблокирующим raycast
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0.6f;
                _canvasGroup.blocksRaycasts = false;
            }

            // Перемещаем в корень Canvas для правильного отображения поверх других элементов
            transform.SetParent(transform.root, true);
            transform.SetAsLastSibling();

            Debug.Log($"Started dragging {_keyColor} key");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            // Обновляем позицию объекта
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            _isDragging = false;

            // Восстанавливаем прозрачность и raycast
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
            }

            // Проверяем, был ли drop на замок
            bool droppedOnLock = false;

            // Получаем объект под курсором
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                var lockHandler = result.gameObject.GetComponent<LockDropComponent>();
                if (lockHandler != null)
                {
                    // Пытаемся сбросить ключ на замок
                    if (lockHandler.TryDropKey(_keyColor))
                    {
                        droppedOnLock = true;
                        Debug.Log($"Successfully dropped {_keyColor} key on lock!");

                        // Уничтожаем ключ после успешного drop
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        Debug.Log($"Failed to drop {_keyColor} key on lock - wrong color or already collected");
                    }
                    break;
                }
            }

            // Если не удалось сбросить на замок, возвращаем ключ на место
            if (!droppedOnLock)
            {
                transform.SetParent(_originalParent, true);
                transform.position = _originalPosition;
                Debug.Log($"Returned {_keyColor} key to original position");
            }
        }
    }
}
