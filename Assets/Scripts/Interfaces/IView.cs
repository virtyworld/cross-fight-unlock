using UnityEngine;
using UnityEngine.Events;

namespace CrossFightUnlock.Interfaces
{
    /// <summary>
    /// Базовый интерфейс для всех представлений в MVP паттерне
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Инициализация представления
        /// </summary>
        void Initialize();

        /// <summary>
        /// Показать представление
        /// </summary>
        void Show();

        /// <summary>
        /// Скрыть представление
        /// </summary>
        void Hide();

        /// <summary>
        /// Очистка ресурсов представления
        /// </summary>
        void Cleanup();
    }
}
