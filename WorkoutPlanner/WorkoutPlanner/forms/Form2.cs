using System;
using System.Drawing;
using System.Windows.Forms;
using WorkoutPlanner.Models;
using WorkoutPlanner.Services;

namespace WorkoutPlanner
{
    public partial class Form2 : Form
    {
        private readonly WorkoutService _workoutService;

        public Form2(WorkoutService workoutService)
        {
            InitializeComponent();
            _workoutService = workoutService;
            InitializeForm();
        }

        private void InitializeForm()
        {
            
            numericUpDown1.Value = 3;  // Подходы
            numericUpDown2.Value = 10; // Повторения
            numericUpDown3.Value = 0;  // Вес

            
            SetupPlaceholder(textBox1, "Введите название упражнения");
            SetupPlaceholder(textBox2, "Дополнительные заметки (необязательно)");
        }

        private void SetupPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            if (ValidateForm())
            {
                SaveExercise();
            }
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                textBox1.Text == "Введите название упражнения")
            {
                MessageBox.Show("Введите название упражнения", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            if (numericUpDown1.Value <= 0)
            {
                MessageBox.Show("Количество подходов должно быть больше 0", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numericUpDown1.Focus();
                return false;
            }

            if (numericUpDown2.Value <= 0)
            {
                MessageBox.Show("Количество повторений должно быть больше 0", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numericUpDown2.Focus();
                return false;
            }

            if (numericUpDown3.Value < 0)
            {
                MessageBox.Show("Вес не может быть отрицательным", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numericUpDown3.Focus();
                return false;
            }

            return true;
        }

        private void SaveExercise()
        {
            try
            {
                var exercise = new CompletedExercise
                {
                    Name = textBox1.Text.Trim(),
                    Sets = (int)numericUpDown1.Value,
                    Reps = (int)numericUpDown2.Value,
                    Weight = numericUpDown3.Value,
                    Notes = textBox2.Text == "Дополнительные заметки (необязательно)" ?
                           string.Empty : textBox2.Text.Trim(),
                    IsCompleted = true,
                    CompletionDate = DateTime.Now
                };
                _workoutService.AddCompletedExercise(exercise);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
