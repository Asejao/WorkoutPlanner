using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WorkoutPlanner.Models
{
    [Serializable] // Атрибут для возможности сериализации в XML
    public class WorkoutPlan
    {
        // Уникальный идентификатор плана тренировки
        public int Id { get; set; }

        // Название плана тренировки
        public string Name { get; set; }

        // Описание плана тренировки
        public string Description { get; set; }

        // Список упражнений в плане тренировки
        // Инициализация пустым списком для избежания NullReferenceException
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

        // Переопределение метода ToString для красивого отображения
        public override string ToString()
        {
            // Формат: "Название (количество упражнений)"
            return $"{Name} ({Exercises.Count} упражнений)";
        }
    }
}