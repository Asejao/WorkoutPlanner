using System;
using System.Xml.Serialization;

namespace WorkoutPlanner.Models
{
    [Serializable]
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal Weight { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Sets}x{Reps} ({Weight}кг)";
        }
    }
}
