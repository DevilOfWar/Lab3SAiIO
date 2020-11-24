using System;
using System.Collections.Generic;

namespace Lab3SAiIO
{
    public class Table
    {
        public double[][] Matrix;
        private double[][] _oldMatrix;
        public Table(IReadOnlyList<double[]> matrix)
        {
            Matrix = new double[matrix.Count][];
            for (var indexFirst = 0; indexFirst < matrix.Count; indexFirst++)
            {
                Matrix[indexFirst] = new double[matrix[0].Length];
                for (var indexSecond = 0; indexSecond < matrix[0].Length; indexSecond++)
                {
                    Matrix[indexFirst][indexSecond] = matrix[indexFirst][indexSecond];
                }
            }
            _oldMatrix = new double[matrix.Count][];
            for (var indexFirst = 0; indexFirst < matrix.Count; indexFirst++)
            {
                _oldMatrix[indexFirst] = new double[matrix[0].Length];
                for (var indexSecond = 0; indexSecond < matrix[0].Length; indexSecond++)
                {
                    _oldMatrix[indexFirst][indexSecond] = matrix[indexFirst][indexSecond];
                }
            }
        }
        public void ResetOldMatrix()
        {
            _oldMatrix = new double[Matrix.Length][];
            for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
            {
                _oldMatrix[indexFirst] = new double[Matrix[0].Length];
                for (var indexSecond = 0; indexSecond < Matrix[0].Length; indexSecond++)
                {
                    _oldMatrix[indexFirst][indexSecond] = Matrix[indexFirst][indexSecond];
                }
            }
        }
        private Table(int line, int column)
        {
            Matrix = new double[line][];
            for (var index = 0; index < line; index++)
            {
                Matrix[index] = new double[column];
            }
        }
        public void ExtendLine()
        {
            var newTable = new Table(Matrix.Length + 1, Matrix[0].Length);
            for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
            {
                for (var indexSecond = 0; indexSecond < Matrix[0].Length; indexSecond++)
                {
                    newTable.Matrix[indexFirst][indexSecond] = Matrix[indexFirst][indexSecond];
                }
            }
            Matrix = newTable.Matrix;
        }
        public void ExtendColumn()
        {
            var newTable = new Table(Matrix.Length, Matrix[0].Length + 1);
            for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
            {
                for (var indexSecond = 0; indexSecond < Matrix[0].Length; indexSecond++)
                {
                    newTable.Matrix[indexFirst][indexSecond] = Matrix[indexFirst][indexSecond];
                }
            }
            Matrix = newTable.Matrix;
        }
        public int FindMin()
        {
            var columnIndex = -1;
            for (var index = 0; index < Matrix[^1].Length - 1; index++)
            {
                if (!(Matrix[^1][index] < 0)) continue;
                if (columnIndex < 0 || Matrix[^1][index] < Matrix[^1][columnIndex])
                {
                    columnIndex = index;
                }
            }
            return columnIndex;
        }
        public int FindMinDual()
        {
            var lineIndex = -1;
            for (var index = 0; index < Matrix.Length - 1; index++)
            {
                if (!(Matrix[index][^1] < 0)) continue;
                if (lineIndex < 0 || Matrix[index][^1] < Matrix[lineIndex][^1])
                {
                    lineIndex = index;
                }
            }
            return lineIndex;
        }
        public void Simplex(int[] basic, out int[] basicOut)
        {
            var columnIndex = FindMin();
            while (columnIndex != -1)
            {
                Console.WriteLine("Cимплекс-таблица:");
                Console.Write("|    |");
                for (var index = 0; index < Matrix[0].Length; index++)
                {
                    if (index != Matrix[0].Length - 1)
                    {
                        Console.Write("| X" + index + " |");
                    }
                    else
                    {
                        Console.Write("| B  |");
                    }
                }
                Console.WriteLine();
                for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
                {
                    if (indexFirst != Matrix.Length - 1)
                    {
                        Console.Write("| X" + basic[indexFirst] + " |");
                    }
                    else
                    {
                        Console.Write("|F(X)|");
                    }
                    for (var indexSecond = 0; indexSecond < Matrix[indexFirst].Length; indexSecond++)
                    {
                        Console.Write("|{0,4}|", Matrix[indexFirst][indexSecond]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("Ведущий столбец: " + (columnIndex + 1));
                var coeff = new double[Matrix.Length];
                for (var index = 0; index < Matrix.Length - 1; index++)
                {
                    coeff[index] = Matrix[index][^1] / Matrix[index][columnIndex];
                }
                Console.Write("|coef|");
                for (var index = 0; index < Matrix.Length - 1; index++)
                {
                    Console.Write("|{0,4}|", coeff[index]);
                }
                Console.WriteLine();
                var lineIndex = -1;
                for (var index = 0; index < coeff.Length; index++)
                {
                    if (!(coeff[index] > 0)) continue;
                    if (lineIndex < 0 || coeff[index] < coeff[lineIndex])
                    {
                        lineIndex = index;
                    }
                }

                if (lineIndex < 0)
                {
                    Console.WriteLine("Нет решений.");
                    basicOut = basic;
                    return;
                }
                Console.WriteLine("Ведущая строка: " + (lineIndex + 1));
                basic[lineIndex] = columnIndex;
                var mainElement = Matrix[lineIndex][columnIndex];
                foreach (var t in Matrix)
                {
                    t[columnIndex] = 0;
                }
                Matrix[lineIndex][columnIndex] = mainElement;
                for (var index = 0; index < Matrix[0].Length; index++)
                {
                    Matrix[lineIndex][index] /= mainElement;
                }
                for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
                {
                    for (var indexSecond = 0; indexSecond < Matrix[0].Length; indexSecond++)
                    {
                        if (indexFirst != lineIndex && indexSecond != columnIndex)
                        {
                            Matrix[indexFirst][indexSecond] -= _oldMatrix[indexFirst][columnIndex] *
                                _oldMatrix[lineIndex][indexSecond] / mainElement;
                        }
                    }
                }
                ResetOldMatrix();
                columnIndex = FindMin();
            }
            Console.WriteLine("Симплекс-метод выполнился.");
            Console.WriteLine("Новая симплекс-таблица:");
            Console.Write("|    |");
            for (var index = 0; index < Matrix[0].Length; index++)
            {
                if (index != Matrix[0].Length - 1)
                {
                    Console.Write("| X" + index + " |");
                }
                else
                {
                    Console.Write("| B  |");
                }
            }
            Console.WriteLine();
            for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
            {
                if (indexFirst != Matrix.Length - 1)
                {
                    Console.Write("| X" + basic[indexFirst] + " |");
                }
                else
                {
                    Console.Write("|F(X)|");
                }
                for (var indexSecond = 0; indexSecond < Matrix[indexFirst].Length; indexSecond++)
                {
                    Console.Write("|{0,4}|", Matrix[indexFirst][indexSecond]);
                }
                Console.WriteLine();
            }
            basicOut = basic;
        }
        public void DualSimplex(int[] basic, out int[] basicOut)
        {
            var lineIndex = FindMinDual();
            while (lineIndex != -1)
            {
                Console.WriteLine("Cимплекс-таблица:");
                Console.Write("|    |");
                for (var index = 0; index < Matrix[0].Length; index++)
                {
                    if (index != Matrix[0].Length - 1)
                    {
                        Console.Write("| X" + index + " |");
                    }
                    else
                    {
                        Console.Write("| B  |");
                    }
                }
                Console.WriteLine();
                for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
                {
                    if (indexFirst != Matrix.Length - 1)
                    {
                        Console.Write("| X" + basic[indexFirst] + " |");
                    }
                    else
                    {
                        Console.Write("|F(X)|");
                    }
                    for (var indexSecond = 0; indexSecond < Matrix[indexFirst].Length; indexSecond++)
                    {
                        Console.Write("|{0,4}|", Matrix[indexFirst][indexSecond]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("Ведущая строка: " + (lineIndex + 1));
                var coeff = new double[Matrix[lineIndex].Length - 1];
                for (var index = 0; index < Matrix[lineIndex].Length - 1; index++)
                {
                    coeff[index] = Matrix[^1][index] / Matrix[lineIndex][index];
                }
                Console.Write("|coef|");
                for (var index = 0; index < Matrix[0].Length - 1; index++)
                {
                    Console.Write("|{0,4}|", coeff[index]);
                }
                Console.WriteLine();
                var columnIndex = -1;
                for (var index = 0; index < coeff.Length; index++)
                {
                    if (!(coeff[index] > 0)) continue;
                    if (columnIndex < 0 || coeff[index] < coeff[columnIndex])
                    {
                        columnIndex = index;
                    }
                }

                if (columnIndex < 0)
                {
                    Console.WriteLine("Нет решений.");
                    basicOut = basic;
                    return;
                }
                Console.WriteLine("Ведущий столбец: " + (columnIndex + 1));
                basic[lineIndex] = columnIndex;
                var mainElement = Matrix[lineIndex][columnIndex];
                foreach (var t in Matrix)
                {
                    t[columnIndex] = 0;
                }
                Matrix[lineIndex][columnIndex] = mainElement;
                for (var index = 0; index < Matrix[0].Length; index++)
                {
                    Matrix[lineIndex][index] /= mainElement;
                }
                for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
                {
                    for (var indexSecond = 0; indexSecond < Matrix[0].Length; indexSecond++)
                    {
                        if (indexFirst != lineIndex && indexSecond != columnIndex)
                        {
                            Matrix[indexFirst][indexSecond] -= _oldMatrix[indexFirst][columnIndex] *
                                _oldMatrix[lineIndex][indexSecond] / mainElement;
                        }
                    }
                }
                ResetOldMatrix();
                lineIndex = FindMinDual();
            }
            Console.WriteLine("Симплекс-метод выполнился.");
            Console.WriteLine("Новая симплекс-таблица:");
            Console.Write("|    |");
            for (var index = 0; index < Matrix[0].Length; index++)
            {
                if (index != Matrix[0].Length - 1)
                {
                    Console.Write("| X" + index + " |");
                }
                else
                {
                    Console.Write("| B  |");
                }
            }
            Console.WriteLine();
            for (var indexFirst = 0; indexFirst < Matrix.Length; indexFirst++)
            {
                if (indexFirst != Matrix.Length - 1)
                {
                    Console.Write("| X" + basic[indexFirst] + " |");
                }
                else
                {
                    Console.Write("|F(X)|");
                }
                for (var indexSecond = 0; indexSecond < Matrix[indexFirst].Length; indexSecond++)
                {
                    Console.Write("|{0,4}|", Matrix[indexFirst][indexSecond]);
                }
                Console.WriteLine();
            }
            basicOut = basic;
        }
    }
}