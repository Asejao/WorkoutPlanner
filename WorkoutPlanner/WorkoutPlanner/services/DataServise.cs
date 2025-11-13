using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Services
{
    public class DataService
    {
        // Путь к папке с данными (рядом с исполняемым файлом)
        private readonly string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        // Конструктор - создает папку для данных если ее нет
        public DataService()
        {
            if (!Directory.Exists(_dataPath))
                Directory.CreateDirectory(_dataPath); // Создание папки если не существует
        }

        // Сохранение списка планов тренировок в XML файл
        public void SaveWorkoutPlans(List<WorkoutPlan> plans)
        {
            try
            {
                string filePath = Path.Combine(_dataPath, "workout_plans.xml");
                using (var writer = new StreamWriter(filePath))
                {
                    // Создание сериализатора для типа List<WorkoutPlan>
                    var serializer = new XmlSerializer(typeof(List<WorkoutPlan>));
                    // Сериализация данных в XML и запись в файл
                    serializer.Serialize(writer, plans);
                }
            }
            catch (Exception ex)
            {
                // Преобразование исключения в более информативное
                throw new Exception($"Ошибка сохранения планов тренировок: {ex.Message}");
            }
        }

        // Загрузка списка планов тренировок из XML файла
        public List<WorkoutPlan> LoadWorkoutPlans()
        {
            try
            {
                string filePath = Path.Combine(_dataPath, "workout_plans.xml");
                // Проверка существования файла
                if (!File.Exists(filePath)) return new List<WorkoutPlan>();

                using (var reader = new StreamReader(filePath))
                {
                    var serializer = new XmlSerializer(typeof(List<WorkoutPlan>));
                    // Десериализация XML в объект List<WorkoutPlan>
                    return (List<WorkoutPlan>)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                // В случае ошибки возвращаем пустой список
                return new List<WorkoutPlan>();
            }
        }

        // Сохранение списка выполненных упражнений в XML файл
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

        // Загрузка списка выполненных упражнений из XML файла
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