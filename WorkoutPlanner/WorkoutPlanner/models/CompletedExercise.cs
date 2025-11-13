using System;
using System.Xml.Serialization;

namespace WorkoutPlanner.Models
{
    [Serializable] // Атрибут для возможности сериализации в XML
    public class CompletedExercise : Exercise // Наследование от базового класса Exercise
    {
        // Дата и время выполнения упражнения
        public DateTime CompletionDate { get; set; }

        // Флаг, указывающий выполнено ли упражнение
        public bool IsCompleted { get; set; }

        // Дополнительные заметки к упражнению
        public string Notes { get; set; }

        // Переопределение метода ToString для красивого отображения
        public override string ToString()
        {
            // Формат: "дата время: название - подходыxповторения (вескг)"
            return $"{CompletionDate:dd.MM.yyyy HH:mm}: {Name} - {Sets}x{Reps} ({Weight}кг)";
        }
    }
}