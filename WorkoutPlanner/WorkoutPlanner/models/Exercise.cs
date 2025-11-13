using System;
using System.Xml.Serialization;

namespace WorkoutPlanner.Models
{
    [Serializable] // Атрибут для возможности сериализации в XML
    public class Exercise
    {
        // Уникальный идентификатор упражнения
        public int Id { get; set; }

        // Название упражнения
        public string Name { get; set; }

        // Количество подходов
        public int Sets { get; set; }

        // Количество повторений в подходе
        public int Reps { get; set; }

        // Вес снаряда в килограммах
        public decimal Weight { get; set; }

        // Описание упражнения (техника выполнения и т.д.)
        public string Description { get; set; }

        // Переопределение метода ToString для красивого отображения
        public override string ToString()
        {
            // Формат: "название - подходыxповторения (вескг)"
            return $"{Name} - {Sets}x{Reps} ({Weight}кг)";
        }
    }
}