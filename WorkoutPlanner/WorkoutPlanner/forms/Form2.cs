using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutPlanner.Models;
using WorkoutPlanner.Services;

namespace WorkoutPlanner
{
    public partial class Form2 : Form
    {
        // Сервис для работы с данными тренировок
        private readonly WorkoutService _workoutService;

        // Конструктор формы добавления упражнения
        public Form2(WorkoutService workoutService)
        {
            // Инициализация компонентов формы
            InitializeComponent();
            // Сохранение ссылки на сервис
            _workoutService = workoutService;
            // Настройка начального состояния формы
            InitializeForm();
        }

        // Инициализация состояния формы
        private void InitializeForm()
        {
            // Установка начальных значений для числовых полей
            numericUpDown1.Value = 3;  // Подходы (значение по умолчанию)
            numericUpDown2.Value = 10; // Повторения (значение по умолчанию)
            numericUpDown3.Value = 0;  // Вес (значение по умолчанию)

            // Настройка плейсхолдеров для текстовых полей
            SetupPlaceholder(textBox1, "Введите название упражнения");
            SetupPlaceholder(textBox2, "Дополнительные заметки (необязательно)");
        }

        // Метод настройки плейсхолдера для текстового поля
        private void SetupPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder; // Установка текста плейсхолдера
            textBox.ForeColor = Color.Gray; // Серый цвет для плейсхолдера

            // Обработчик события получения фокуса полем
            textBox.Enter += (s, e) =>
            {
                // Если текст равен плейсхолдеру, очищаем поле
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black; // Черный цвет для обычного текста
                }
            };

            // Обработчик события потери фокуса полем
            textBox.Leave += (s, e) =>
            {
                // Если поле пустое, восстанавливаем плейсхолдер
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray; // Серый цвет для плейсхолдера
                }
            };
        }

        // Обработчик нажатия кнопки "Сохранить"
        private void button1_Click(object sender, EventArgs e)
        {
            // Проверка валидности данных перед сохранением
            if (ValidateForm())
            {
                SaveExercise(); // Сохранение упражнения
            }
        }

        // Обработчик нажатия кнопки "Отмена"
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Установка результата диалога
            this.Close(); // Закрытие формы
        }

        // Метод валидации данных формы
        private bool ValidateForm()
        {
            // Проверка названия упражнения
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                textBox1.Text == "Введите название упражнения")
            {
                MessageBox.Show("Введите название упражнения", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus(); // Установка фокуса на поле с ошибкой
                return false;
            }

            // Проверка количества подходов
            if (numericUpDown1.Value <= 0)
            {
                MessageBox.Show("Количество подходов должно быть больше 0", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numericUpDown1.Focus();
                return false;
            }

            // Проверка количества повторений
            if (numericUpDown2.Value <= 0)
            {
                MessageBox.Show("Количество повторений должно быть больше 0", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numericUpDown2.Focus();
                return false;
            }

            // Проверка веса (не может быть отрицательным)
            if (numericUpDown3.Value < 0)
            {
                MessageBox.Show("Вес не может быть отрицательным", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numericUpDown3.Focus();
                return false;
            }

            return true; // Все проверки пройдены
        }

        // Метод сохранения упражнения
        private void SaveExercise()
        {
            try
            {
                // Создание объекта выполненного упражнения
                var exercise = new CompletedExercise
                {
                    Name = textBox1.Text.Trim(), // Название (удаляем лишние пробелы)
                    Sets = (int)numericUpDown1.Value, // Количество подходов
                    Reps = (int)numericUpDown2.Value, // Количество повторений
                    Weight = numericUpDown3.Value, // Вес
                    // Обработка заметок (если это плейсхолдер - сохраняем пустую строку)
                    Notes = textBox2.Text == "Дополнительные заметки (необязательно)" ?
                           string.Empty : textBox2.Text.Trim(),
                    IsCompleted = true, // Отметка о выполнении
                    CompletionDate = DateTime.Now // Текущая дата и время
                };

                // Добавление упражнения через сервис
                _workoutService.AddCompletedExercise(exercise);

                // Установка успешного результата и закрытие формы
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Обработка ошибок при сохранении
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}