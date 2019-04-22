using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TaskForForecsys
{
    /// <summary>
    /// класс для расширающий методы стандартного string[]
    /// </summary>
    public static class arrayExtension
    {
        /// <summary>
        /// добавление в массив значения
        /// </summary>
        /// <param name="array"></param>
        /// <param name="newValue"></param>
        /// <returns>новый массив</returns>
        public static string[] Add(this string[] array, string newValue)
        {
            try
            {
                int arrLength = (array == null) ? 0 : array.Length;
                int newLength = arrLength + 1;

                string[] result = new string[newLength];

                for (int i = 0; i < arrLength; i++)
                    result[i] = array[i];

                result[newLength - 1] = newValue;

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return new string[] { };
        }

        /// <summary>
        /// удаление по значению
        /// </summary>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string[] Remove(this string[] array, string item)
        {
            try
            {
                int remInd = -1;

                for (int i = 0; i < array.Length; ++i)
                {
                    if (array[i] == item)
                    {
                        remInd = i;
                        break;
                    }
                }
                string[] retVal = new string[array.Length - 1];

                for (int i = 0, j = 0; i < retVal.Length; ++i, ++j)
                {
                    if (j == remInd)
                        ++j;

                    retVal[i] = array[j];
                }

                return retVal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return new string[] { };
        }

        /// <summary>
        /// проверка на вхождение элемента в массив
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns>true - если содержит, false - нет</returns>
        public static bool Cointains(this string[] array, string value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    /// <summary>
    /// класс для парсинга .txt файла в формате id;count;Country
    /// </summary>
    class Parser
    {
        #region Свойства
        private string[] user_id, count, country;

        private FileInfo fi;
        private string reg = @"[;\n\r]";
        #endregion

        #region Методы

        /// <summary>
        /// конструктор
        /// </summary>
        /// <param name="fileName">имя файла, из которого будем читать</param>
        public Parser(string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName))
                fi = new FileInfo(@"..\..\..\inp.txt");
            else
                fi = new FileInfo(fileName);

            readFile();
        }

        /// <summary>
        /// сумма по count
        /// </summary>
        public int countSum(string country)
        {
            int sum = 0;
            try
            {
                for (int i = 0; i < count.Length; i++)
                    if (country.Equals(this.country[i]))
                        sum += Convert.ToInt32(count[i]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return sum;
        }

        /// <summary>
        /// число уникальных user_id для country
        /// </summary>
        public int countUniq(string country)
        {
            string[] uniq_id = { };

            for (int i = 0; i < user_id.Length; i++)
            {
                if (this.country[i].Equals(country))
                    if (!uniq_id.Contains(user_id[i]))
                        uniq_id = uniq_id.Add(user_id[i]);
                    else
                        uniq_id = uniq_id.Remove(user_id[i]);
            }
            return uniq_id.Length;
        }

        /// <summary>
        /// вспомогательная функция для получения массива уникальных 
        /// значений из неупорядоченного массива
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public string[] getUniqFromArr(string[] arr)
        {
            string[] uniq_country = { };

            for (int i = 0; i < arr.Length; i++)
            {
                if (!uniq_country.Cointains(arr[i]))
                    uniq_country = uniq_country.Add(arr[i]);
            }
            return uniq_country;
        }

        /// <summary>
        /// Вывод на консоль статистики по странам из файла
        /// </summary>
        public void printCountryStatistic()
        {
            string[] uniq_country = getUniqFromArr(this.country);
            try
            {
                for (int i = 0; i < uniq_country.Length; i++)
                {
                    int countC = countSum(uniq_country[i]);
                    int countU = countUniq(uniq_country[i]);

                    Console.WriteLine(
                        "Страна: " + uniq_country[i] +
                        "; Сумма по count: " + Convert.ToString(countC) +
                        "; Число уникальных user_id для country: " + Convert.ToString(countU) + ";\n"
                    );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
        }

        /// <summary>
        /// чтение данных из файла
        /// </summary>
        /// <returns></returns>
        private string readFile()
        {
            string content = "";
            using (StreamReader _sw = new StreamReader(fi.FullName, Encoding.Default))
            {
                string line;
                while ((line = _sw.ReadLine()) != null)
                {
                    dataFactory(line);
                }
            }
            return content;
        }

        /// <summary>
        /// заполняем массивы данными из файла
        /// </summary>
        private bool dataFactory(string line)
        {
            var parsedArr = Regex.Split(line, reg);
            if (parsedArr.Length != 3) return false;

            for (int i = 0, flag = 1; i < parsedArr.Length; i++, flag++)
            {
                if (flag > 3) flag = 1;

                if (flag == 1)
                    user_id = user_id.Add(parsedArr[i]);
                else if (flag == 2)
                    count = count.Add(parsedArr[i]);
                else if (flag == 3)
                    country = country.Add(parsedArr[i]);
            }
            return true;
        }
        #endregion
    }
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Parser p = new Parser();
            p.printCountryStatistic();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("Тайминг: " + elapsedTime);
        }
    }
}
