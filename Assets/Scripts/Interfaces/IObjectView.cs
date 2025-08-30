using UnityEngine;

namespace CrossFightUnlock.Interfaces
{
    /// <summary>
    /// Интерфейс для представления объекта, движущегося по линии
    /// </summary>
    public interface IObjectView
    {
        /// <summary>
        /// Инициализация представления
        /// </summary>
        void Initialize();

        /// <summary>
        /// Установка позиции
        /// </summary>
        void SetPosition(Vector3 position);

        /// <summary>
        /// Движение в заданном направлении с заданной скоростью
        /// </summary>
        void Move(Vector3 direction, float speed);

        /// <summary>
        /// Остановка движения
        /// </summary>
        void Stop();

        /// <summary>
        /// Показать представление
        /// </summary>
        void Show();

        /// <summary>
        /// Скрыть представление
        /// </summary>
        void Hide();

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Получить текущую позицию
        /// </summary>
        Vector3 GetPosition();

        /// <summary>
        /// Проверить, активно ли представление
        /// </summary>
        bool IsActive();

        /// <summary>
        /// Установить направление движения
        /// </summary>
        void SetMovementDirection(Vector3 direction);

        /// <summary>
        /// Установить скорость движения
        /// </summary>
        void SetMovementSpeed(float speed);
    }
}
