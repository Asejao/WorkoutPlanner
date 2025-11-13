using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WorkoutPlanner.Services;

namespace WorkoutPlanner
{
    public partial class Form1 : Form
    {
        private readonly WorkoutService _workoutService;

        public Form1()
        {
            InitializeComponent(); // Вызов автоматически сгенерированного метода
            _workoutService = new WorkoutService();
            InitializeData();
        }

        private void InitializeData()
        {
            LoadWorkoutPlans();
            LoadActivityHistory();
        }

        private void LoadWorkoutPlans()
        {
            listBox1.Items.Clear(); // Планы тренировок
            var plans = _workoutService.GetWorkoutPlans();

            foreach (var plan in plans)
            {
                listBox1.Items.Add(plan);
            }

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        private void LoadActivityHistory()
        {
            listBox3.Items.Clear(); // История активности
            var history = _workoutService.GetActivityHistory();

            foreach (var exercise in history)
            {
                listBox3.Items.Add(exercise);
            }

            // Обновляем статистику
            UpdateHistoryStats();
        }

        private void UpdateHistoryStats()
        {
            var history = _workoutService.GetActivityHistory();
            int totalExercises = history.Count;

            // Обновляем заголовок вкладки с количеством записей
            tabPage2.Text = $"История активности ({totalExercises})";
        }

        private void LoadExercisesForSelectedPlan()
        {
            listBox2.Items.Clear(); // Упражнения плана

            if (listBox1.SelectedItem is Models.WorkoutPlan selectedPlan)
            {
                foreach (var exercise in selectedPlan.Exercises)
                {
                    listBox2.Items.Add(exercise);
                }
            }
        }

        // Обработчики событий
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExercisesForSelectedPlan();
        }

        private void button1_Click(object sender, EventArgs e) // Добавить упражнение
        {
            Form2 addForm = new Form2(_workoutService);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadActivityHistory();
                MessageBox.Show("Упражнение успешно добавлено в историю!", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e) // Обновить
        {
            InitializeData();
        }

        private void button3_Click(object sender, EventArgs e) // Очистить историю
        {
            ClearHistory();
        }

        private void ClearHistory()
        {
            var history = _workoutService.GetActivityHistory();

            if (history.Count == 0)
            {
                MessageBox.Show("История активности уже пуста!", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Подтверждение очистки с показом статистики
            var result = MessageBox.Show(
                $"Вы уверены, что хотите полностью очистить историю?\n\n" +
                $"Будет удалено: {history.Count} упражнений\n" +
                $"Последняя запись: {history.First().CompletionDate:dd.MM.yyyy HH:mm}",
                "Подтверждение очистки истории",
MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Вызываем метод очистки истории в сервисе
                    _workoutService.ClearActivityHistory();

                    // Обновляем отображение
                    LoadActivityHistory();

                    MessageBox.Show($"История активности успешно очищена!\nУдалено записей: {history.Count}",
                                  "Успех",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при очистке истории: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }

        // Новая функция: очистка истории за определенный период
        private void ClearHistoryByDateRange()
        {
            var history = _workoutService.GetActivityHistory();

            if (history.Count == 0)
            {
                MessageBox.Show("История активности пуста!", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Используем простой диалог вместо отдельной формы
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;

            var exercisesToDelete = history
                .Where(e => e.CompletionDate >= startDate && e.CompletionDate <= endDate)
                .ToList();

            if (exercisesToDelete.Count == 0)
            {
                MessageBox.Show("За последний месяц записей не найдено!", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить все записи за последний месяц?\n\n" +
                $"Будет удалено: {exercisesToDelete.Count} упражнений",
                "Очистка истории за период",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                _workoutService.ClearActivityHistoryByDateRange(startDate, endDate);
                LoadActivityHistory();

                MessageBox.Show($"Записи за последний месяц удалены!\nУдалено: {exercisesToDelete.Count} упражнений",
                              "Успех",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2) // Вкладка истории
            {
                LoadActivityHistory();
            }
        }

        // Контекстное меню для истории
        private void listBox3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Получаем выбранный элемент
                int index = listBox3.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    listBox3.SelectedIndex = index;

                    // Создаем контекстное меню
                    ContextMenuStrip contextMenu = new ContextMenuStrip();

                    var deleteItem = new ToolStripMenuItem("Удалить эту запись");
                    deleteItem.Click += (s, args) => DeleteSelectedHistoryItem();

                    var clearAllItem = new ToolStripMenuItem("Очистить всю историю");
                    clearAllItem.Click += (s, args) => ClearHistory();

                    var clearByDateItem = new ToolStripMenuItem("Очистить за последний месяц");
                    clearByDateItem.Click += (s, args) => ClearHistoryByDateRange();

                    contextMenu.Items.Add(deleteItem);
                    contextMenu.Items.Add(new ToolStripSeparator());
                    contextMenu.Items.Add(clearAllItem);
                    contextMenu.Items.Add(clearByDateItem);

                    contextMenu.Show(listBox3, e.Location);
                }
            }
        }

        private void DeleteSelectedHistoryItem()
        {
            if (listBox3.SelectedItem is Models.CompletedExercise selectedExercise)
            {
                var result = MessageBox.Show(
                    $"Удалить запись: {selectedExercise.Name}?\n" +
                    $"Дата: {selectedExercise.CompletionDate:dd.MM.yyyy HH:mm}",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _workoutService.DeleteHistoryItem(selectedExercise.Id);
                    LoadActivityHistory();

                    MessageBox.Show("Запись удалена!", "Успех",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}





