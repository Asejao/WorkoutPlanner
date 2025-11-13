using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WorkoutPlanner.Services;

namespace WorkoutPlanner
{
    public partial class Form1 : Form
    {
        // Сервис для работы с данными тренировок
        private readonly WorkoutService _workoutService;

        // Конструктор главной формы
        public Form1()
        {
            // Инициализация компонентов формы (созданных в дизайнере)
            InitializeComponent();
            // Создание экземпляра сервиса для работы с данными
            _workoutService = new WorkoutService();
            // Загрузка начальных данных
            InitializeData();
        }

        // Инициализация данных при запуске формы
        private void InitializeData()
        {
            LoadWorkoutPlans();    // Загрузка планов тренировок
            LoadActivityHistory(); // Загрузка истории активности
        }

        // Загрузка списка планов тренировок в listBox1
        private void LoadWorkoutPlans()
        {
            listBox1.Items.Clear(); // Очистка списка перед загрузкой
            var plans = _workoutService.GetWorkoutPlans(); // Получение планов из сервиса

            // Добавление каждого плана в список
            foreach (var plan in plans)
            {
                listBox1.Items.Add(plan);
            }

            // Автоматический выбор первого элемента, если список не пуст
            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        // Загрузка истории выполненных упражнений в listBox3
        private void LoadActivityHistory()
        {
            listBox3.Items.Clear(); // Очистка списка перед загрузкой
            var history = _workoutService.GetActivityHistory(); // Получение истории из сервиса

            // Добавление каждого упражнения в список истории
            foreach (var exercise in history)
            {
                listBox3.Items.Add(exercise);
            }

            // Обновление статистики (количества записей)
            UpdateHistoryStats();
        }

        // Обновление статистики истории активности
        private void UpdateHistoryStats()
        {
            var history = _workoutService.GetActivityHistory();
            int totalExercises = history.Count; // Подсчет общего количества упражнений

            // Обновление заголовка вкладки с отображением количества записей
            tabPage2.Text = $"История активности ({totalExercises})";
        }

        // Загрузка упражнений выбранного плана тренировок в listBox2
        private void LoadExercisesForSelectedPlan()
        {
            listBox2.Items.Clear(); // Очистка списка упражнений

            // Проверка, что выбранный элемент является планом тренировки
            if (listBox1.SelectedItem is Models.WorkoutPlan selectedPlan)
            {
                // Добавление каждого упражнения из выбранного плана
                foreach (var exercise in selectedPlan.Exercises)
                {
                    listBox2.Items.Add(exercise);
                }
            }
        }

        // Обработчик события изменения выбранного плана тренировок
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExercisesForSelectedPlan(); // Загрузка упражнений для нового выбранного плана
        }

        // Обработчик нажатия кнопки "Добавить упражнение"
        private void button1_Click(object sender, EventArgs e)
        {
            // Создание и отображение формы добавления упражнения
            Form2 addForm = new Form2(_workoutService);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                // Если упражнение успешно добавлено, обновляем историю
                LoadActivityHistory();
                MessageBox.Show("Упражнение успешно добавлено в историю!", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Обработчик нажатия кнопки "Обновить"
        private void button2_Click(object sender, EventArgs e)
        {
            InitializeData(); // Полная перезагрузка данных
        }

        // Обработчик нажатия кнопки "Очистить историю"
        private void button3_Click(object sender, EventArgs e)
        {
            ClearHistory(); // Вызов метода очистки истории
        }

        // Метод полной очистки истории активности
        private void ClearHistory()
        {
            var history = _workoutService.GetActivityHistory();

            // Проверка, есть ли что очищать
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
                MessageBoxDefaultButton.Button2 // По умолчанию выбрана кнопка "Нет"
            );

            // Если пользователь подтвердил очистку
            if (result == DialogResult.Yes)
            {
                try
                {
                    // Вызов метода очистки истории в сервисе
                    _workoutService.ClearActivityHistory();

                    // Обновление отображения
                    LoadActivityHistory();

                    MessageBox.Show($"История активности успешно очищена!\nУдалено записей: {history.Count}",
                                  "Успех",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Обработка ошибок при очистке
                    MessageBox.Show($"Ошибка при очистке истории: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
        }

        // Метод очистки истории за определенный период (последний месяц)
        private void ClearHistoryByDateRange()
        {
            var history = _workoutService.GetActivityHistory();

            // Проверка, есть ли записи в истории
            if (history.Count == 0)
            {
                MessageBox.Show("История активности пуста!", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Установка диапазона дат (последний месяц)
            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;

            // Фильтрация упражнений за указанный период
            var exercisesToDelete = history
                .Where(e => e.CompletionDate >= startDate && e.CompletionDate <= endDate)
                .ToList();

            // Проверка, есть ли записи за указанный период
            if (exercisesToDelete.Count == 0)
            {
                MessageBox.Show("За последний месяц записей не найдено!", "Информация",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Подтверждение удаления
            var result = MessageBox.Show(
                $"Удалить все записи за последний месяц?\n\n" +
                $"Будет удалено: {exercisesToDelete.Count} упражнений",
                "Очистка истории за период",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Если пользователь подтвердил удаление
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

        // Обработчик события переключения вкладок
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Если выбрана вкладка истории, обновляем ее содержимое
            if (tabControl1.SelectedTab == tabPage2)
            {
                LoadActivityHistory();
            }
        }

        // Обработчик события нажатия правой кнопки мыши на списке истории
        private void listBox3_MouseDown(object sender, MouseEventArgs e)
        {
            // Проверка, что нажата правая кнопка мыши
            if (e.Button == MouseButtons.Right)
            {
                // Определение индекса элемента под курсором
                int index = listBox3.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches) // Если элемент найден
                {
                    listBox3.SelectedIndex = index; // Выделение элемента

                    // Создание контекстного меню
                    ContextMenuStrip contextMenu = new ContextMenuStrip();

                    // Пункт меню для удаления одной записи
                    var deleteItem = new ToolStripMenuItem("Удалить эту запись");
                    deleteItem.Click += (s, args) => DeleteSelectedHistoryItem();

                    // Пункт меню для полной очистки истории
                    var clearAllItem = new ToolStripMenuItem("Очистить всю историю");
                    clearAllItem.Click += (s, args) => ClearHistory();

                    // Пункт меню для очистки за последний месяц
                    var clearByDateItem = new ToolStripMenuItem("Очистить за последний месяц");
                    clearByDateItem.Click += (s, args) => ClearHistoryByDateRange();

                    // Добавление пунктов в меню
                    contextMenu.Items.Add(deleteItem);
                    contextMenu.Items.Add(new ToolStripSeparator()); // Разделитель
                    contextMenu.Items.Add(clearAllItem);
                    contextMenu.Items.Add(clearByDateItem);

                    // Отображение контекстного меню
                    contextMenu.Show(listBox3, e.Location);
                }
            }
        }

        // Метод удаления выбранной записи из истории
        private void DeleteSelectedHistoryItem()
        {
            // Проверка, что выбранный элемент является выполненным упражнением
            if (listBox3.SelectedItem is Models.CompletedExercise selectedExercise)
            {
                // Подтверждение удаления
                var result = MessageBox.Show(
                    $"Удалить запись: {selectedExercise.Name}?\n" +
                    $"Дата: {selectedExercise.CompletionDate:dd.MM.yyyy HH:mm}",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                // Если пользователь подтвердил удаление
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