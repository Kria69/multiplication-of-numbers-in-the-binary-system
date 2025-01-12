﻿using System;

class Program
{
    // Проверка на введения числа
    static ushort GetBinary(string hexNumber)
    {
        if (string.IsNullOrWhiteSpace(hexNumber))
        {
            throw new ArgumentException("Не введено никакое число");
        }
        if (hexNumber.StartsWith("-"))
        {
            throw new ArgumentException("Ввод отрицательного числа не допускается");
        }
        if (hexNumber.Any(c => IsCyrillic(c)))
        {
            throw new ArgumentException("Ввод киррилицы запрещен");
        }
        ushort number = Convert.ToUInt16(hexNumber, 16);
        if (number > 0xFF)
        {
            throw new ArgumentException("Введенное число превышает 1 байт");
        }
        return number;
    }
    static bool IsCyrillic(char c)
    {
        return c >= 'А' && c <= 'я';
    }

    static ushort BinaryMultiply(ushort bin1, ushort bin2, string operation)
    {
        ushort SCHP = 0;
        bin1 &= 0xFFFF;
        // умножения с анализом младшего разряда множителя со сдвигом множимого
        if (operation == "junior")
        {
            Console.WriteLine("  Step  |       bin1       |   bin2   |       SCHP");
            Console.WriteLine("--------------------------------------------------------");

            int length = GetMultiplierLength(bin2);
            for (int i = 0; i < length + 1; i++)
            {
                Console.WriteLine($"   {i,-5}| " +
                    $"{Convert.ToString(bin1, 2).PadLeft(16, '0')} | " +
                    $"{Convert.ToString(bin2, 2).PadLeft(length, '0')} | " +
                    $"{Convert.ToString(SCHP, 2).PadLeft(16, '0')}");                
                if ((bin2 & 1) != 0)
                {
                    SCHP += bin1;
                }
                bin1 <<= 1;
                bin1 &= 0xFFFF;
                bin2 >>= 1;
            }
        }
        // умножения с анализом старшего разряда множителя со сдвигом мномимого
        else if (operation == "older")
        {
            Console.WriteLine("  Step  |       bin1       |   bin2   |       SCHP");
            Console.WriteLine("--------------------------------------------------------");

            int length = GetMultiplierLength(bin2);
            bin1 <<= length;
            bin1 &= 0xFFFF;
            bin2 &= (ushort)(length <= 4 ? 0xF : 0xFF);

            for (int i = 0; i < length + 1; i++)
            {
                Console.WriteLine($"   {i,-5}| " +
                    $"{Convert.ToString(bin1, 2).PadLeft(16, '0')} | " +
                    $"{Convert.ToString(bin2, 2).PadLeft(length, '0')} | " +
                    $"{Convert.ToString(SCHP, 2).PadLeft(16, '0')}");
                bin1 >>= 1;
                if ((bin2 & (length <= 4 ? 0b1000 : 0b10000000)) != 0)
                {
                    SCHP += bin1;
                }
                bin2 <<= 1;
                bin2 &= (ushort)(length <= 4 ? 0xF : 0xFF);
            }
        }
        // умножения с анализом младшего разряда множителя со сдвигом СЧП
        else if (operation == "junior_SCHP")
        {
            Console.WriteLine("  Step  |       bin1       |   bin2   |       SCHP");
            Console.WriteLine("--------------------------------------------------------");

            int length = GetMultiplierLength(bin2);
            bin2 &= (ushort)(length <= 4 ? 0xF : 0xFF);

            for (int i = 0; i < length; i++)
            {
                Console.WriteLine($"   {i,-5}| " +
                    $"{Convert.ToString(bin1, 2).PadLeft(length, '0')} | " +
                    $"{Convert.ToString(bin2, 2).PadLeft(length, '0')} | " +
                    $"{Convert.ToString(SCHP, 2).PadLeft(16, '0')}");

                if ((bin2 & 1) != 0)
                {
                    SCHP |= (ushort)((length <= 4 ? bin1 << 4 : bin1 << 8));
                }

                SCHP >>= 1;
                bin2 >>= 1;
            }
        }
        // умножения с анализом старшего разряда множителя со сдвигом СЧП
        else if (operation == "older_SCHP")
        {
            Console.WriteLine("  Step  |       bin1       |   bin2   |       SCHP");
            Console.WriteLine("--------------------------------------------------------");

            int length = GetMultiplierLength(bin2);
            bin2 &= (ushort)(length <= 4 ? 0xF : 0xFF);

            for (int i = 0; i < length + 1; i++)
            {
                Console.WriteLine($"   {i,-5}| " +
                    $"{Convert.ToString(bin1, 2).PadLeft(length, '0')} | " +
                    $"{Convert.ToString(bin2, 2).PadLeft(length, '0')} | " +
                    $"{Convert.ToString(SCHP, 2).PadLeft(16, '0')}");

                if ((bin2 & (length <= 4 ? 0b1000 : 0b10000000)) != 0)
                {
                    SCHP += bin1;
                }

                if (i < length - 1)
                {
                    SCHP <<= 1;
                }

                bin2 <<= 1;
                bin2 &= (ushort)(length <= 4 ? 0xF : 0xFF);
            }
        }

        return SCHP;
    }

// Вычисление длины множителя
    static int GetMultiplierLength(ushort bin2)
    {
        int length = 0;
        for (int i = 15; i >= 0; i--)
        {
            if ((bin2 & (1 << i)) != 0)
            {
                length = i + 1;
                break;
            }
        }

        length = (length + 3) / 4 * 4;

        return Math.Clamp(length, 4, 8);
    }

    static void Main(string[] args)
    {
        // Обработка входящих чисел
        string[] numbers = new string[2];
        for (int i = 0; i < 2; i++)
        {
            Console.Write($"Введите {(i == 0 ? "первое" : "второе")} число в шестнадцатеричной системе: ");
            numbers[i] = Console.ReadLine();
        }

        ushort[] binaryNumbers = new ushort[2];
        try
        {
            for (int i = 0; i < 2; i++)
            {
                binaryNumbers[i] = GetBinary(numbers[i]);
            }
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Ошибка ввода: " + e.Message);
            return;
        }

        // Вывод входящих чисел на экран в двух системах
        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine($"{(i == 0 ? "Первое" : "Второе")} число в 16 системе счисления: {numbers[i].ToUpper()}h");
            Console.WriteLine($"{(i == 0 ? "Первое" : "Второе")} число в двоичной системе: {Convert.ToString(binaryNumbers[i], 2).PadLeft(8, '0')}b");
        }

        Console.WriteLine();

        // Вывод результатов умножения
        string[] operations = { "junior", "older", "junior_SCHP", "older_SCHP" };
        string[] descriptions = {
            "Результат умножения в 16 системе счисления с анализом младшего разряда множителя со сдвигом множимого",
            "Результат умножения в 16 системе счисления с анализом старшего разряда множителя со сдвигом множимого",
            "Результат умножения в 16 системе счисления с анализом младшего разряда множителя со сдвигом СЧП",
            "Результат умножения в 16 системе счисления с анализом старшего разряда множителя со сдвигом СЧП"
        };

        for (int i = 0; i < operations.Length; i++)
        {
            Console.WriteLine("\n" + new string('=', 56));
            ushort ALU = BinaryMultiply(binaryNumbers[0], binaryNumbers[1], operations[i]);
            string ALU_hex = ALU.ToString("X").PadLeft(4, '0');
            Console.WriteLine($"{descriptions[i]}: {ALU_hex.Substring(0)}h");
        }
    }
}
