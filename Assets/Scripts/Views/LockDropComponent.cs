using UnityEngine;
using UnityEngine.EventSystems;

namespace CrossFightUnlock.Views
{
    public class LockDropComponent : MonoBehaviour, IDropHandler
    {
        private KeyColor _lockColor;
        private CrossFightUnlock.Presenters.UnlockPresenter _unlockPresenter;

        public KeyColor LockColor => _lockColor;

        public void Initialize(KeyColor lockColor, CrossFightUnlock.Presenters.UnlockPresenter unlockPresenter)
        {
            _lockColor = lockColor;
            _unlockPresenter = unlockPresenter;
        }

        public void OnDrop(PointerEventData eventData)
        {
            // Этот метод вызывается автоматически при drop на объект
            // Логика обработки drop уже реализована в KeyDragComponent
        }

        public bool TryDropKey(KeyColor keyColor)
        {
            if (_unlockPresenter == null) return false;

            // Проверяем совпадение цветов
            if (keyColor == _lockColor)
            {
                // Пытаемся добавить ключ через presenter
                return _unlockPresenter.TryDropKey(keyColor);
            }

            return false;
        }
    }
}
