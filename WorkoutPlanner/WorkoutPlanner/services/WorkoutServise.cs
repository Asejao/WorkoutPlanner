using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Services
{
    public class WorkoutService
    {
        private readonly DataService _dataService;
        private List<WorkoutPlan> _workoutPlans;
        private List<CompletedExercise> _completedExercises;

        public WorkoutService()
        {
            _dataService = new DataService();
            LoadData();
            InitializeSampleData();
        }

        private void LoadData()
        {
            _workoutPlans = _dataService.LoadWorkoutPlans();
            _completedExercises = _dataService.LoadCompletedExercises();
        }

        private void InitializeSampleData()
        {
            if (_workoutPlans.Count == 0)
            {
                var strengthWorkout = new WorkoutPlan
                {
                    Id = 1,
                    Name = "Силовая тренировка",
                    Description = "Базовая силовая тренировка на все тело"
                };

                strengthWorkout.Exercises.AddRange(new[]
                {
                    new Exercise { Id = 1, Name = "Приседания со штангой", Sets = 3, Reps = 10, Weight = 50 },
                    new Exercise { Id = 2, Name = "Жим лежа", Sets = 4, Reps = 8, Weight = 40 },
                    new Exercise { Id = 3, Name = "Тяга штанги в наклоне", Sets = 3, Reps = 10, Weight = 30 }
                });

                var cardioWorkout = new WorkoutPlan
                {
                    Id = 2,
                    Name = "Кардио тренировка",
                    Description = "Интервальная кардио тренировка"
                };

                cardioWorkout.Exercises.AddRange(new[]
                {
                    new Exercise { Id = 4, Name = "Беговая дорожка", Sets = 1, Reps = 20, Weight = 0 },
                    new Exercise { Id = 5, Name = "Велотренажер", Sets = 1, Reps = 15, Weight = 0 }
                });

                _workoutPlans.Add(strengthWorkout);
                _workoutPlans.Add(cardioWorkout);
                SaveWorkoutPlans();
            }
        }

        // Основные методы
        public List<WorkoutPlan> GetWorkoutPlans()
        {
            return _workoutPlans;
        }

        public void AddCompletedExercise(CompletedExercise exercise)
        {
            exercise.Id = _completedExercises.Count > 0 ? _completedExercises.Max(e => e.Id) + 1 : 1;
            exercise.CompletionDate = DateTime.Now;
            _completedExercises.Add(exercise);
            _dataService.SaveCompletedExercises(_completedExercises);
        }

        public List<CompletedExercise> GetActivityHistory()
        {
            return _completedExercises.OrderByDescending(e => e.CompletionDate).ToList();
        }

        public void SaveWorkoutPlans()
        {
            _dataService.SaveWorkoutPlans(_workoutPlans);
        }

        // Методы для очистки истории
        public void ClearActivityHistory()
        {
            _completedExercises.Clear();
            _dataService.SaveCompletedExercises(_completedExercises);
        }

        public void ClearActivityHistoryByDateRange(DateTime startDate, DateTime endDate)
        {
            _completedExercises.RemoveAll(e =>
                e.CompletionDate >= startDate && e.CompletionDate <= endDate);
            _dataService.SaveCompletedExercises(_completedExercises);
        }

        public void DeleteHistoryItem(int exerciseId)
        {
            _completedExercises.RemoveAll(e => e.Id == exerciseId);
            _dataService.SaveCompletedExercises(_completedExercises);
        }

        public void KeepOnlyRecentExercises(int keepCount)
        {
            if (_completedExercises.Count > keepCount)
            {
                var exercisesToKeep = _completedExercises
                    .OrderByDescending(e => e.CompletionDate)
                    .Take(keepCount)
                    .ToList();

                _completedExercises = exercisesToKeep;
                _dataService.SaveCompletedExercises(_completedExercises);
            }
        }
    }
}
