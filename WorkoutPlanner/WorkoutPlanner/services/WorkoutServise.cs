using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Services
{
    public class WorkoutService
    {
        // Сервис для работы с хранилищем данных
        private readonly DataService _dataService;
        // Списки данных в памяти
        private List<WorkoutPlan> _workoutPlans;
        private List<CompletedExercise> _completedExercises;

        // Конструктор - инициализация данных
        public WorkoutService()
        {
            _dataService = new DataService();
            LoadData();           // Загрузка данных из хранилища
            InitializeSampleData(); // Создание примеров данных если нужно
        }

        // Загрузка данных из хранилища
        private void LoadData()
        {
            _workoutPlans = _dataService.LoadWorkoutPlans();
            _completedExercises = _dataService.LoadCompletedExercises();
        }

        // Инициализация примерных данных если хранилище пустое
        private void InitializeSampleData()
        {
            if (_workoutPlans.Count == 0)
            {
                // Создание силовой тренировки
                var strengthWorkout = new WorkoutPlan
                {
                    Id = 1,
                    Name = "Силовая тренировка",
                    Description = "Базовая силовая тренировка на все тело"
                };

                // Добавление упражнений в силовую тренировку
                strengthWorkout.Exercises.AddRange(new[]
                {
                    new Exercise { Id = 1, Name = "Приседания со штангой", Sets = 3, Reps = 10, Weight = 50 },
                    new Exercise { Id = 2, Name = "Жим лежа", Sets = 4, Reps = 8, Weight = 40 },
                    new Exercise { Id = 3, Name = "Тяга штанги в наклоне", Sets = 3, Reps = 10, Weight = 30 }
                });

                // Создание кардио тренировки
                var cardioWorkout = new WorkoutPlan
                {
                    Id = 2,
                    Name = "Кардио тренировка",
                    Description = "Интервальная кардио тренировка"
                };

                // Добавление упражнений в кардио тренировку
                cardioWorkout.Exercises.AddRange(new[]
                {
                    new Exercise { Id = 4, Name = "Беговая дорожка", Sets = 1, Reps = 20, Weight = 0 },
                    new Exercise { Id = 5, Name = "Велотренажер", Sets = 1, Reps = 15, Weight = 0 }
                });

                // Добавление планов в общий список
                _workoutPlans.Add(strengthWorkout);
                _workoutPlans.Add(cardioWorkout);
                // Сохранение созданных планов
                SaveWorkoutPlans();
            }
        }

        // Основные методы

        // Получение списка всех планов тренировок
        public List<WorkoutPlan> GetWorkoutPlans()
        {
            return _workoutPlans;
        }

        // Добавление выполненного упражнения в историю
        public void AddCompletedExercise(CompletedExercise exercise)
        {
            // Генерация ID для нового упражнения
            exercise.Id = _completedExercises.Count > 0 ? _completedExercises.Max(e => e.Id) + 1 : 1;
            exercise.CompletionDate = DateTime.Now; // Установка текущей даты и времени
            _completedExercises.Add(exercise); // Добавление в список
            _dataService.SaveCompletedExercises(_completedExercises); // Сохранение в хранилище
        }

        // Получение истории активности (сортировка по дате - новые сверху)
        public List<CompletedExercise> GetActivityHistory()
        {
            return _completedExercises.OrderByDescending(e => e.CompletionDate).ToList();
        }

        // Сохранение планов тренировок в хранилище
        public void SaveWorkoutPlans()
        {
            _dataService.SaveWorkoutPlans(_workoutPlans);
        }

        // Методы для очистки истории

        // Полная очистка истории активности
        public void ClearActivityHistory()
        {
            _completedExercises.Clear(); // Очистка списка в памяти
            _dataService.SaveCompletedExercises(_completedExercises); // Сохранение пустого списка
        }

        // Очистка истории за указанный период дат
        public void ClearActivityHistoryByDateRange(DateTime startDate, DateTime endDate)
        {
            // Удаление всех упражнений, попадающих в диапазон дат
            _completedExercises.RemoveAll(e =>
                e.CompletionDate >= startDate && e.CompletionDate <= endDate);
            _dataService.SaveCompletedExercises(_completedExercises);
        }

        // Удаление конкретной записи по ID
        public void DeleteHistoryItem(int exerciseId)
        {
            // Удаление упражнения с указанным ID
            _completedExercises.RemoveAll(e => e.Id == exerciseId);
            _dataService.SaveCompletedExercises(_completedExercises);
        }

        // Сохранение только последних N записей
        public void KeepOnlyRecentExercises(int keepCount)
        {
            if (_completedExercises.Count > keepCount)
            {
                // Выборка N самых свежих записей
                var exercisesToKeep = _completedExercises
                    .OrderByDescending(e => e.CompletionDate)
                    .Take(keepCount)
                    .ToList();

                // Замена всего списка на отфильтрованный
                _completedExercises = exercisesToKeep;
                _dataService.SaveCompletedExercises(_completedExercises);
            }
        }
    }
}