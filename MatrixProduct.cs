using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MatrixProducrThreadPool
{
    /// <summary>
    /// Класс, отвечающий за умножение матриц.
    /// Матрица представлена как двумерный массив типа double.
    /// Поддерживает как однопоточное, так и многопоточное (с использованием ThreadPool) умножение.
    /// </summary>
    public class MatrixProduct
    {
        /// <summary>
        /// Публичный конструктор по умолчанию.
        /// Не выполняет никаких действий, так как класс содержит только статические методы.
        /// </summary>
        public MatrixProduct() { }

        /// <summary>
        /// Выполняет умножение двух матриц с использованием пула потоков (ThreadPool).
        /// Каждая строка результирующей матрицы вычисляется в отдельном потоке из пула.
        /// </summary>
        /// <param name="matr1">Первая матрица-множитель (размерность: m x n).</param>
        /// <param name="matr2">Вторая матрица-множитель (размерность: n x p).</param>
        /// <returns>Результирующая матрица размерности m x p.</returns>
        /// <exception cref="ArgumentException">
        /// Возникает, если количество столбцов первой матрицы не совпадает с количеством строк второй матрицы,
        /// что делает матричное умножение невозможным.
        /// </exception>
        static public double[,] multiplyParallel(double[,] matr1, double[,] matr2)
        {
            // Проверка корректности размерностей матриц для умножения
            if (!_canMultiply(matr1, matr2))
            {
                throw new ArgumentException("Количество столбцов первой матрицы должно быть равно количеству строк второй");
            }

            // Создание результирующей матрицы с размерностью: [строки matr1] x [столбцы matr2]
            double[,] result = new double[matr1.GetLength(0), matr2.GetLength(1)];

            // Создание объекта синхронизации, который будет ожидать завершения всех потоков.
            // Изначально счётчик равен количеству строк первой матрицы (по одному потоку на строку).
            CountdownEvent countdown = new CountdownEvent(matr1.GetLength(0));

            // Запуск потока для вычисления каждой строки результата
            for (int i = 0; i < matr1.GetLength(0); i++)
            {
                // Захватываем текущий индекс строки в локальную переменную,
                // чтобы избежать захвата изменяющейся переменной цикла в замыкании.
                int row = i;

                // Добавляем задачу в пул потоков
                ThreadPool.QueueUserWorkItem(state =>
                {
                    // Вычисление всех элементов строки 'row' результирующей матрицы
                    for (int k = 0; k < matr2.GetLength(0); k++) // k — индекс столбца matr1 / строки matr2
                    {
                        for (int j = 0; j < matr2.GetLength(1); j++) // j — индекс столбца matr2
                        {
                            // Накопление произведения элементов: matr1[row, k] * matr2[k, j]
                            result[row, j] += matr1[row, k] * matr2[k, j];
                        }
                    }

                    // Уведомляем объект синхронизации, что текущий поток завершил работу
                    countdown.Signal();
                });
            }

            // Основной поток ожидает, пока все рабочие потоки не завершат вычисления
            countdown.Wait();

            // Возвращаем готовую результирующую матрицу
            return result;
        }

        /// <summary>
        /// Выполняет умножение двух матриц в одном потоке (последовательно).
        /// Используется стандартный алгоритм умножения матриц O(n³).
        /// </summary>
        /// <param name="matr1">Первая матрица-множитель (размерность: m x n).</param>
        /// <param name="matr2">Вторая матрица-множитель (размерность: n x p).</param>
        /// <returns>Результирующая матрица размерности m x p.</returns>
        /// <exception cref="ArgumentException">
        /// Возникает, если количество столбцов первой матрицы не совпадает с количеством строк второй матрицы.
        /// </exception>
        static public double[,] multiply(double[,] matr1, double[,] matr2)
        {
            // Проверка возможности умножения
            if (!_canMultiply(matr1, matr2))
            {
                throw new ArgumentException("Количество столбцов первой матрицы должно быть равно количеству строк второй");
            }

            // Инициализация результирующей матрицы нулями
            double[,] result = new double[matr1.GetLength(0), matr2.GetLength(1)];

            // i — строка результата (и matr1)
            // k — общий индекс (столбец matr1 / строка matr2)
            // j — столбец результата (и matr2)
            for (int i = 0; i < matr1.GetLength(0); i++)
            {
                for (int k = 0; k < matr2.GetLength(0); k++)
                {
                    for (int j = 0; j < matr2.GetLength(1); j++)
                    {
                        // Накопление суммы произведений
                        result[i, j] += matr1[i, k] * matr2[k, j];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Проверяет, возможно ли умножить две заданные матрицы.
        /// Условие: количество столбцов первой матрицы должно равняться количеству строк второй.
        /// </summary>
        /// <param name="matr1">Первая матрица.</param>
        /// <param name="matr2">Вторая матрица.</param>
        /// <returns>
        /// true — если умножение возможно; 
        /// false — в противном случае.
        /// </returns>
        static private bool _canMultiply(double[,] matr1, double[,] matr2)
        {
            // GetLength(1) — количество столбцов первой матрицы
            // GetLength(0) — количество строк второй матрицы
            return matr1.GetLength(1) == matr2.GetLength(0);
        }
    }
}