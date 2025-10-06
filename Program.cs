// Stopwatch
using System.Diagnostics;
using System.Security.Cryptography;

namespace MatrixProducrThreadPool
{
    /// <summary>
    /// Основной класс программы, содержащий точку входа Main и вспомогательные методы.
    /// Предназначен для генерации случайных матриц и сравнения производительности
    /// однопоточного и многопоточного умножения матриц.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Генерирует двумерную матрицу заданного размера со случайными значениями.
        /// </summary>
        /// <param name="rows">Количество строк в матрице.</param>
        /// <param name="cols">Количество столбцов в матрице.</param>
        /// <param name="min">Минимальное значение элемента (включительно).</param>
        /// <param name="max">Максимальное значение элемента (исключительно).</param>
        /// <param name="precision">Количество знаков после запятой (округление).</param>
        /// <returns>Двумерный массив double с заданными параметрами.</returns>
        static double[,] randomMatrix(int rows, int cols, int min, int max, int precision)
        {
            // Создание новой матрицы указанного размера
            double[,] matrix = new double[rows, cols];

            // Инициализация генератора псевдослучайных чисел
            Random rand = new Random();

            // Вычисление коэффициента для округления 
            int coef = (int)Math.Pow(10, precision);

            // Заполнение матрицы случайными значениями
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Генерация случайного числа в диапазоне [min, max)
                    // rand.NextDouble() возвращает значение от 0  до 1
                    double randomValue = min + (max - min) * rand.NextDouble();

                    // Округление до указанного количества знаков после запятой
                    matrix[i, j] = Math.Round(randomValue, precision); 
                }
            }

            // Возврат сгенерированной матрицы
            return matrix;
        }

        /// <summary>
        /// Точка входа в программу.
        /// Запускает бесконечный цикл, в котором пользователь вводит размер квадратных матриц,
        /// после чего программа генерирует две случайные матрицы и сравнивает время
        /// выполнения однопоточного и многопоточного умножения.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        static void Main(string[] args)
        {
            // Бесконечный цикл для многократного запуска тестов без перезапуска программы
            while (true)
            {
                // Запрос у пользователя размера квадратных матриц
                Console.WriteLine("Введите размер матриц");

                // Чтение ввода пользователя и преобразование в целое число
                int size = int.Parse(Console.ReadLine());

                // Генерация двух квадратных матриц размером size x size
                // Значения в диапазоне [-10000, 10000), округлённые до целых (precision = 0)
                double[,] matrix = randomMatrix(size, size, -10000, 10000, 0);
                double[,] matrix1 = randomMatrix(size, size, -10000, 10000, 0);

                // Создание и запуск таймера для измерения времени однопоточного умножения
                var sw = new Stopwatch();
                sw.Start();

                // Вызов однопоточного метода умножения
                MatrixProduct.multiply(matrix, matrix1);

                // Остановка таймера
                sw.Stop();

                // Вывод времени выполнения однопоточного умножения
                Console.WriteLine($"Умножение в одном потоке: {sw.Elapsed}");

                // опять таймер
                var sw1 = Stopwatch.StartNew();

                // Вызов многопоточного метода умножения 
                MatrixProduct.multiplyParallel(matrix, matrix1);

                // Остановка второго таймера
                sw1.Stop();

                // Вывод времени выполнения многопоточного умножения
                Console.WriteLine($"Умножение в пуле потоков: {sw1.Elapsed}");

                // Программа возвращается к началу цикла — ожидает новый ввод размера
            }
        }
    }
}