using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lab10
{
    class Program
    {
        static public void Main(string[] args)
        {
            string inputFileName = "dust.csv";
            string outputFileName = "generated_dust.csv";

            Dust[] generatedDustArray = null; // Инициализация переменной
            Stopwatch stopwatch = null; // Инициализация переменной

            Console.WriteLine("Добро пожаловать в программу!\n");

            while (true)
            {
                Console.WriteLine("1. Провести генерацию");
                Console.WriteLine("2. Вывести информацию об образцах");
                Console.WriteLine("3. Сохранить данные в файл");
                Console.WriteLine("4. Вывести информацию о скорости работы программы");
                Console.WriteLine("5. Выход");

                Console.Write("\nВыберите действие: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.Clear();

                        Dust[] dustArray;

                        while (true)
                        {
                            Console.Write("Введите название файла (в формате название_файла.csv): ");
                            inputFileName = Console.ReadLine();

                            Console.WriteLine($"Производится чтение файла {inputFileName}...");
                            dustArray = ReadCsvFile(inputFileName); // Чтение файла через метод

                            if (dustArray.Length != 0)
                            {
                                break;
                            }
                        }

                        Console.WriteLine("Вычисляется статистика...");
                        var statistics = GetStatistics(dustArray); // Вычисление статистики

                        Console.WriteLine("Генерация запущена...");

                        stopwatch = Stopwatch.StartNew(); // Начало измерения скорости работы программы

                        generatedDustArray = GenerateDust(1000000, statistics); // Генерация данных

                        stopwatch.Stop(); // Окончание измерения скорости работы программы

                        Console.Clear();
                        Console.WriteLine($"Генерация успешно завершена!\n");

                        break;

                    case "2":
                        Console.Clear();

                        if (generatedDustArray == null)
                        {
                            Console.WriteLine("Ошибка: данные не были сгенерированы. Выберите пункт 1 для генерации данных.\n");
                            break;
                        }

                        int numberOfSamples;
                        while (true)
                        {
                            Console.Write("Введите количество образцов, которые желаете увидеть: ");
                            if (!int.TryParse(Console.ReadLine(), out numberOfSamples))
                            {
                                Console.WriteLine("Ошибка: введено не число!");
                            }
                            else break;
                        }

                        Console.Clear();

                        Console.WriteLine($"Данные о {numberOfSamples} образцах: ");

                        PrintDust(generatedDustArray, numberOfSamples); // Вывод информации об образцах

                        Console.WriteLine("Вернуться в меню? ");

                        Console.ReadKey();

                        Console.Clear();
                        break;
                    case "3":

                        Console.Clear();

                        if (generatedDustArray == null)
                        {
                            Console.WriteLine("Ошибка: данные не были сгенерированы. Выберите пункт 1 для генерации данных.\n");
                            break;
                        }

                        Console.Clear();

                        while (true)
                        {
                            Console.Write("Введите название файла (в формате название_файла.csv): ");
                            outputFileName = Console.ReadLine();

                            if (outputFileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)) // Проверка на окончание .csv
                            {
                                if (string.Equals(outputFileName, inputFileName, StringComparison.OrdinalIgnoreCase)) // Регистронезависимое сравнение
                                {
                                    Console.Clear();
                                    Console.WriteLine("Ошибка: имя файла совпадает с именем входного файла. Попробуйте снова.");
                                }
                                else
                                {
                                    break; // Выходим из цикла, если проверка пройдена
                                }
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Ошибка: имя файла не оканчивается на .csv. Попробуйте снова.");
                            }
                        }

                        WriteCsvFile(outputFileName, generatedDustArray); // Запись данных в файл

                        Console.Clear();

                        Console.WriteLine($"Данные успешно сохранены в файл: {outputFileName}\n");

                        break;
                    case "4":
                        Console.Clear();

                        if (generatedDustArray == null)
                        {
                            Console.WriteLine("Ошибка: данные не были сгенерированы. Выберите пункт 1 для генерации данных.\n");
                            break;
                        }

                        Console.WriteLine($"Время выполнения программы: {stopwatch.ElapsedMilliseconds} мс.\n");

                        break;
                    case "5":
                        Console.WriteLine("Выход из программы...");
                        return;
                    default:
                        Console.Clear();
                        Console.WriteLine("Ошибка: неизвестное действие. Попробуйте снова.\n");
                        break;
                }
            }
        }
        static public Dust[] ReadCsvFile(string fileName)
        {
            var dustList = new List<Dust>();
            try
            {
                using (var reader = new StreamReader(fileName))
                {

                    string header = reader.ReadLine(); // Пропускаем заголовок

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';'); // Используем разделитель - точка с запятой

                        // Проверка длины строки
                        if (values.Length < 10)
                        {
                            Console.WriteLine($"Пропущена строка: {line} (недостаточно столбцов)");
                            continue;
                        }

                        // Замена запятых на точки и обработка пустых значений
                        string GetValue(string input) => string.IsNullOrWhiteSpace(input) ? "0.0" : input.Replace(",", ".");

                        try
                        {
                            dustList.Add(new Dust
                            {
                                Resistivity = double.Parse(GetValue(values[0]), CultureInfo.InvariantCulture),
                                Temperature = double.Parse(GetValue(values[1]), CultureInfo.InvariantCulture),
                                Humidity = double.Parse(GetValue(values[2]), CultureInfo.InvariantCulture),
                                Density = double.Parse(GetValue(values[3]), CultureInfo.InvariantCulture),
                                DustCapacity = double.Parse(GetValue(values[4]), CultureInfo.InvariantCulture),
                                ParticleSize = double.Parse(GetValue(values[5]), CultureInfo.InvariantCulture),
                                Conductivity = string.IsNullOrWhiteSpace(values[7]) ? "неизвестно" : values[7],
                                DustDispersiveness = string.IsNullOrWhiteSpace(values[8]) ? "неизвестно" : values[8],
                                Formation = string.IsNullOrWhiteSpace(values[9]) ? "неизвестно" : values[9]
                            });
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"Ошибка формата в строке: {line}. Сообщение: {ex.Message}");
                        }
                    }
                }

                Console.WriteLine($"Обработано строк: {dustList.Count}");
            }
            catch (FileNotFoundException ex)
            {
                Console.Clear();
                Console.WriteLine("Файл не найден! Проверьте название и попробуйте ещё раз!\n");
            }

            return dustList.ToArray();
        }
        static Dictionary<string, object> GetStatistics(Dust[] dustArray)
        {
            var statistics = new Dictionary<string, object>();

            // Числовая статистика
            double[] densities = dustArray.Select(d => d.Density).ToArray();
            statistics["Density"] = new
            {
                Max = densities.Max(),
                Min = densities.Min(),
                Average = densities.Average(),
                Variance = densities.Average(v => Math.Pow(v - densities.Average(), 2)),
                StdDev = Math.Sqrt(densities.Average(v => Math.Pow(v - densities.Average(), 2)))
            };

            // Текстовая статистика
            statistics["Conductivity"] = dustArray.GroupBy(d => d.Conductivity)
                                                  .ToDictionary(g => g.Key, g => g.Count());

            statistics["DustDispersiveness"] = dustArray.GroupBy(d => d.DustDispersiveness)
                                                         .ToDictionary(g => g.Key, g => g.Count());

            statistics["Formation"] = dustArray.GroupBy(d => d.Formation)
                                                .ToDictionary(g => g.Key, g => g.Count());

            return statistics;
        }
        static Dust[] GenerateDust(int count, Dictionary<string, object> statistics)
        {
            var random = new Random();
            var densityStats = (dynamic)statistics["Density"];
            var conductivityDist = (Dictionary<string, int>)statistics["Conductivity"];
            var dustDispersivenessDist = (Dictionary<string, int>)statistics["DustDispersiveness"];
            var formationDist = (Dictionary<string, int>)statistics["Formation"];
            // Локальная функция для генерации значений
            static double[] GenerateScaledValues(int count, Random random, double mean, double min, double max, double spread)
            {
                return Enumerable.Range(0, count)
                    .Select(_ =>
                    {
                        double value;
                        do
                        {
                            // Генерация значения с нормальным распределением
                            value = random.NextGaussian(mean, spread);
                            // Смещение значений ближе к среднему, чтобы избежать превышений
                            value = mean + (value - mean) * 0.8;
                        } while (value < min || value > max); // Повторяем, пока значение не попадёт в диапазон

                        return value;
                    })
                    .ToArray();
            }
            // Генерация значений для параметров
            var temperatureValues = GenerateScaledValues(count, random, 16.875, -20, 25, 5);
            var humidityValues = GenerateScaledValues(count, random, 4.25, 1, 21, 3);
            var densityValues = GenerateScaledValues(count, random, 1225, 755, 2155, 300);
            var dustCapacityValues = GenerateScaledValues(count, random, 2.37, 2.3, 2.42, 0.02);
            var particleSizeValues = GenerateScaledValues(count, random, 30, 0.5, 95, 10);
            var resistivityValues = GenerateScaledValues(count, random, 100_000, 0.001, 3_500_000, 100_000);
            return Enumerable.Range(0, count).Select(i => new Dust
            {
                Temperature = temperatureValues[i],
                Humidity = humidityValues[i],
                Density = densityValues[i],
                DustCapacity = dustCapacityValues[i],
                ParticleSize = particleSizeValues[i],
                Resistivity = resistivityValues[i],
                Conductivity = GetRandomFromDistribution(random, conductivityDist),
                DustDispersiveness = GetRandomFromDistribution(random, dustDispersivenessDist),
                Formation = GetRandomFromDistribution(random, formationDist)
            }).ToArray();
        }
        static string GetRandomFromDistribution(Random random, Dictionary<string, int> distribution)
        {
            int total = distribution.Values.Sum();
            int roll = random.Next(total);
            int cumulative = 0;
            foreach (var kvp in distribution)
            {
                cumulative += kvp.Value;
                if (roll < cumulative)
                    return kvp.Key;
            }
            return distribution.Keys.First();
        }
        static void PrintDust(Dust[] dustArray, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var dust = dustArray[i];
                Console.WriteLine($"{i + 1}. Температура: {dust.Temperature}\n Влажность: {dust.Humidity}\n Плотность: {dust.Density}\n Ёмкость: {dust.DustCapacity}\n Размер частиц: {dust.ParticleSize}\n УЭС: {dust.Resistivity}\n Электропроводность: {dust.Conductivity}\n Дисперсность: {dust.DustDispersiveness}\n Способ образования: {dust.Formation}\n");
            }
        }
        static void WriteCsvFile(string fileName, Dust[] dustArray)
        {
            using (var writer = new StreamWriter(fileName))
            {
                // Заголовок
                writer.WriteLine("resistivity;temperature;humidity;density;dust_capacity;particle_size;conductivity;dust_dispersiveness;formation");

                // Запись строк
                foreach (var dust in dustArray)
                {
                    writer.WriteLine($"{dust.Resistivity};{dust.Temperature};{dust.Humidity};{dust.Density};{dust.DustCapacity};{dust.ParticleSize};{dust.Conductivity};{dust.DustDispersiveness};{dust.Formation}");
                }
            }
        }
    }
}
