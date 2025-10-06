using System;
using Xunit;
using MatrixProducrThreadPool; 

namespace MatrixProductTests
{
    public class MatrixProductTests
    {
        /// <summary>
        /// Сравнивает две матрицы типа double
        /// </summary>
        /// <param name="expected">Ожидаемая матрица (эталон).</param>
        /// <param name="actual">Фактически полученная матрица.</param>
        /// <param name="tolerance">Допустимая погрешность при сравнении элементов (по умолчанию 1e-10).</param>
        /// <remarks>
        /// </remarks>
        private static void AssertMatricesEqual(double[,] expected, double[,] actual, double tolerance = 1e-10)
        {
            // Проверяем, что количество строк в обеих матрицах одинаково.
            // Если нет —  исключение
            Assert.Equal(expected.GetLength(0), actual.GetLength(0));

            // Проверяем, что количество столбцов в обеих матрицах одинаково.
            Assert.Equal(expected.GetLength(1), actual.GetLength(1));

            // Проходим по каждой строке матрицы
            for (int i = 0; i < expected.GetLength(0); i++)
            {
                // Проходим по каждому столбцу текущей строки
                for (int j = 0; j < expected.GetLength(1); j++)
                {
                    // Вычисляем абсолютную разницу между ожидаемым и фактическим значением
                    double difference = Math.Abs(expected[i, j] - actual[i, j]);

                    // Проверяем, что разница не превышает допустимую погрешность tolerance
                    Assert.True(
                        difference <= tolerance,
                        $"Элемент [{i},{j}]: ожидалось {expected[i, j]}, получено {actual[i, j]}"
                    );
                }
            }
        }

        // 2x2 × 2x2
        [Fact]
        public void Multiply_2x2Matrices_ReturnsCorrectResult()
        {
            // Arrange
            double[,] A = { { 1, 2 }, { 3, 4 } };
            double[,] B = { { 5, 6 }, { 7, 8 } };
            double[,] expected = { { 19, 22 }, { 43, 50 } };

            // Act
            var result1 = MatrixProduct.multiply(A, B);
            var result2 = MatrixProduct.multiplyParallel(A, B);

            // Assert
            AssertMatricesEqual(expected, result1);
            AssertMatricesEqual(expected, result2);
        }

        // Тест на исключение при несовместимых размерах
        [Fact]
        public void Multiply_IncompatibleDimensions_ThrowsArgumentException()
        {
            double[,] A = { { 1, 2, 3 } };      // 1x3
            double[,] B = { { 1, 2 }, { 3, 4 } }; // 2x2 несовместимо

            Assert.Throws<ArgumentException>(() => MatrixProduct.multiply(A, B));
            Assert.Throws<ArgumentException>(() => MatrixProduct.multiplyParallel(A, B));
        }

        // Тест с вектором-строкой и вектором-столбцом
        [Fact]
        public void Multiply_RowAndColumnVectors_ReturnsScalar()
        {
            double[,] row = { { 1, 2, 3 } };           // 1x3
            double[,] col = { { 4 }, { 5 }, { 6 } };   // 3x1
            double[,] expected = { { 32 } };           // 1*4 + 2*5 + 3*6 = 32

            var result1 = MatrixProduct.multiply(row, col);
            var result2 = MatrixProduct.multiplyParallel(row, col);

            AssertMatricesEqual(expected, result1);
            AssertMatricesEqual(expected, result2);
        }
    }
}