using System;
using System.Xml.Serialization;

namespace WorkoutPlanner.Models
{
    [Serializable]
    public class CompletedExercise : Exercise
    {
        public DateTime CompletionDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Notes { get; set; }

        public override string ToString()
        {
            return $"{CompletionDate:dd.MM.yyyy HH:mm}: {Name} - {Sets}x{Reps} ({Weight}кг)";
        }
    }
}
