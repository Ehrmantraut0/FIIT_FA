using System;
using System.Linq;
using TreeDataStructures.Core;
using TreeDataStructures.Implementations.BST;

namespace TreeTestDebug
{
    class Program
    {
        static void Main()
        {
            var tree = new BinarySearchTree<int, string>();

            // Добавляем три узла
            tree.Add(10, "Root");
            tree.Add(5, "Left");
            tree.Add(15, "Right");

            Console.WriteLine("=== ТЕСТ ВСЕХ ОБХОДОВ ===");
            Console.WriteLine($"Дерево: 10 (Root), 5 (Left), 15 (Right)\n");

            // ===== СТАНДАРТНЫЕ ОБХОДЫ =====
            Console.WriteLine("--- СТАНДАРТНЫЕ ОБХОДЫ ---");

            // InOrder
            Console.WriteLine("\nInOrder (ЛКП):");
            Console.WriteLine("  Ожидается: 5, 10, 15");
            Console.Write("  Результат: ");
            foreach (var entry in tree.InOrder())
                Console.Write($"{entry.Key} ");
            Console.WriteLine();

            // PreOrder
            Console.WriteLine("\nPreOrder (КЛП):");
            Console.WriteLine("  Ожидается: 10, 5, 15");
            Console.Write("  Результат: ");
            foreach (var entry in tree.PreOrder())
                Console.Write($"{entry.Key} ");
            Console.WriteLine();

            // PostOrder
            Console.WriteLine("\nPostOrder (ЛПК):");
            Console.WriteLine("  Ожидается: 5, 15, 10");
            Console.Write("  Результат: ");
            foreach (var entry in tree.PostOrder())
                Console.Write($"{entry.Key} ");
            Console.WriteLine();

            // ===== REVERSE ОБХОДЫ =====
            Console.WriteLine("\n--- REVERSE ОБХОДЫ ---");

            // InOrderReverse
            Console.WriteLine("\nInOrderReverse (ПКЛ):");
            Console.WriteLine("  Ожидается: 15, 10, 5");
            Console.Write("  Результат: ");
            foreach (var entry in tree.InOrderReverse())
                Console.Write($"{entry.Key} ");
            Console.WriteLine();

            // PreOrderReverse
            Console.WriteLine("\nPreOrderReverse (КПЛ):");
            Console.WriteLine("  Ожидается: 10, 15, 5");
            Console.Write("  Результат: ");
            foreach (var entry in tree.PreOrderReverse())
                Console.Write($"{entry.Key} ");
            Console.WriteLine();

            // PostOrderReverse
            Console.WriteLine("\nPostOrderReverse (ПЛК):");
            Console.WriteLine("  Ожидается: 15, 5, 10");
            Console.Write("  Результат: ");
            foreach (var entry in tree.PostOrderReverse())
                Console.Write($"{entry.Key} ");
            Console.WriteLine();

            // ===== ПРОВЕРКА DEPTH =====
            Console.WriteLine("\n--- ГЛУБИНА УЗЛОВ (InOrder) ---");
            foreach (var entry in tree.InOrder())
                Console.WriteLine($"  Ключ: {entry.Key}, Глубина: {entry.Depth}");

            Console.WriteLine("\n=== ТЕСТ ЗАВЕРШЁН ===");
        }
    }
}