using System;
using System.Linq;

namespace Lab3SAiIO
{
    internal static class Program
    {
        private static void Main()
        {
            double[] functionCoeff = {3, 2, 0};
            var limitationCoeff = new double[3][];
            limitationCoeff[0] = new double[] {1, 1, 13};
            limitationCoeff[1] = new double[] {1, -1, 6};
            limitationCoeff[2] = new double[] {-3, 1, 9};
            var table = new Table(limitationCoeff);
            table.ExtendColumn();
            table.ExtendColumn();
            table.ExtendColumn();
            table.ExtendLine();
            for (var index = 0; index < table.Matrix.Length - 1; index++)
            {
                table.Matrix[index][table.Matrix[index].Length - 1] =
                    table.Matrix[index][limitationCoeff[0].Length - 1];
                table.Matrix[index][limitationCoeff[0].Length - 1] = 0;
            }
            for (var index = 0; index < functionCoeff.Length - 1; index++)
            {
                table.Matrix[^1][index] = -functionCoeff[index];
            }
            for (var index = 0; index < limitationCoeff.Length; index++)
            {
                table.Matrix[index][index + limitationCoeff[0].Length - 1] = 1;
            }
            int[] basic = { 2, 3, 4};
            table.ResetOldMatrix();
            table.Simplex(basic, out basic);
            var simplexResult = new double[functionCoeff.Length - 1];
            var intResult = true;
            for (var index = 0; index < simplexResult.Length; index++)
            {
                simplexResult[index] = basic.Contains(index) ? table.Matrix[basic.ToList().IndexOf(index)][^1] : 0;
                if (Math.Abs(simplexResult[index] - Math.Round(simplexResult[index])) > 0.0001)
                {
                    intResult = false;
                }
            }
            Console.WriteLine("Значения основных переменных: ");
            for (var index = 0; index < simplexResult.Length; index++)
            {
                Console.WriteLine("X" + index + "=" + simplexResult[index]);
            }
            while (!intResult)
            {
                var doubleMaxPartIndex = -1;
                for (var index = 0; index < basic.Length; index++)
                {
                    if (doubleMaxPartIndex < 0 || Math.Abs(table.Matrix[index][^1] - Math.Floor(table.Matrix[index][^1])) >
                        Math.Abs(table.Matrix[doubleMaxPartIndex][^1] - Math.Floor(table.Matrix[doubleMaxPartIndex][^1])))
                    {
                        doubleMaxPartIndex = index;
                    }
                }
                table.ExtendLine();
                table.ExtendColumn();
                for (var index = 0; index < table.Matrix[0].Length; index++)
                {
                    table.Matrix[^1][index] = -table.Matrix[^2][index];
                    table.Matrix[^2][index] = -(table.Matrix[doubleMaxPartIndex][index] -
                                              Math.Floor(table.Matrix[doubleMaxPartIndex][index]));
                }
                for (var index = 0; index < table.Matrix.Length; index++)
                {
                    table.Matrix[index][^1] = table.Matrix[index][^2];
                    table.Matrix[index][^2] = index == table.Matrix.Length - 2 ? 1 : 0;
                }
                table.ResetOldMatrix();
                var basicList = basic.ToList();
                basicList.Add(table.Matrix[0].Length - 2);
                basic = basicList.ToArray();
                table.DualSimplex(basic, out basic);
                intResult = true;
                for (var index = 0; index < simplexResult.Length; index++)
                {
                    simplexResult[index] = basic.Contains(index) ? table.Matrix[basic.ToList().IndexOf(index)][^1] : 0;
                    if (Math.Abs(simplexResult[index] - Math.Round(simplexResult[index])) > 0.0001)
                    {
                        intResult = false;
                    }
                }
                Console.WriteLine("Значения основных переменных: ");
                for (var index = 0; index < simplexResult.Length; index++)
                {
                    Console.WriteLine("X" + index + "=" + simplexResult[index]);
                }
            }
            var functionValue = simplexResult.Select((t, index) => (int) (t * functionCoeff[index])).Sum();

            functionValue += (int)(functionCoeff[^1]);
            Console.WriteLine("Данный оптимальный план выполняет требование целочисленности. Функция равна " +
                              functionValue);
        }
    }
}