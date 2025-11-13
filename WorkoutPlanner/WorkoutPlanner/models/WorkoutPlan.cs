using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WorkoutPlanner.Models
{
    [Serializable]
    public class WorkoutPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

        public override string ToString()
        {
            return $"{Name} ({Exercises.Count} упражнений)";
        }
    }
}
