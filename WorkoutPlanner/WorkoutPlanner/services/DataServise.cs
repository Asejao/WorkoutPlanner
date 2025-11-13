using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Services
{
    public class DataService
    {
        private readonly string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        public DataService()
        {
            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath);
        }

        public void SaveWorkoutPlans(List<WorkoutPlan> plans)
        {
            try
            {
                string filePath = Path.Combine(_dataPath, "workout_plans.xml");
                using (var writer = new StreamWriter(filePath))
                {
                    var serializer = new XmlSerializer(typeof(List<WorkoutPlan>));
                    serializer.Serialize(writer, plans);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения планов тренировок: {ex.Message}");
            }
        }

        public List<WorkoutPlan> LoadWorkoutPlans()
        {
            try
            {
                string filePath = Path.Combine(_dataPath, "workout_plans.xml");
                if (!File.Exists(filePath)) return new List<WorkoutPlan>();

                using (var reader = new StreamReader(filePath))
                {
                    var serializer = new XmlSerializer(typeof(List<WorkoutPlan>));
                    return (List<WorkoutPlan>)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                return new List<WorkoutPlan>();
            }
        }

        public void SaveCompletedExercises(List<CompletedExercise> exercises)
        {
            try
            {
                string filePath = Path.Combine(_dataPath, "completed_exercises.xml");
                using (var writer = new StreamWriter(filePath))
                {
                    var serializer = new XmlSerializer(typeof(List<CompletedExercise>));
                    serializer.Serialize(writer, exercises);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения выполненных упражнений: {ex.Message}");
            }
        }

        public List<CompletedExercise> LoadCompletedExercises()
        {
            try
            {
                string filePath = Path.Combine(_dataPath, "completed_exercises.xml");
                if (!File.Exists(filePath)) return new List<CompletedExercise>();

                using (var reader = new StreamReader(filePath))
                {
                    var serializer = new XmlSerializer(typeof(List<CompletedExercise>));
                    return (List<CompletedExercise>)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                return new List<CompletedExercise>();
            }
        }
    }
}