using UnityEngine;

namespace CrossFightUnlock.Interfaces
{
    /// <summary>
    /// Базовый интерфейс для всех презентеров в MVP паттерне
    /// </summary>
    public interface IPresenter
    {
        /// <summary>
        /// Инициализация презентера
        /// </summary>
        void Initialize();

        /// <summary>
        /// Очистка ресурсов презентера
        /// </summary>
        void Cleanup();
    }
}
