using UnityEngine;

namespace CrossFightUnlock.Interfaces
{
    /// <summary>
    /// Базовый интерфейс для всех моделей в MVP паттерне
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Инициализация модели
        /// </summary>
        void Initialize();

        /// <summary>
        /// Очистка ресурсов модели
        /// </summary>
        void Cleanup();
    }
}
