using System;
using System.Collections.Generic;

struct Box
{
    public int x;
    public int y;
    public int step;
}

class Program
{
    static string[] lab =
    {
        "#########################",
        "    #         #       ###",
        " ## ###### ## # ##### ###",
        " #  #       # #     # ###",
        " ##   # ##### ### # # ###",
        "    ### #   #     # # ###",
        " #### # # ####### # # ###",
        "   #    #   #   # #   ###",
        "## #### ####### # ### ###",
        "      #       # #     ###",
        " #### # ##### ####### ###",
        "       #                 ",
        "#########################"
    };

    static bool[,] visited;
    static Box start;
    static Box finish;
    static int size_x, size_y;
    static Queue<Box> boxes;

    static void load()
    {
        size_x = lab[0].Length;
        size_y = lab.Length;

        start.x = 0;
        start.y = 1;

        finish.x = size_x - 1;
        finish.y = size_y - 2;
    }

    static void print()
    {
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }

        for (int y = 0; y < size_y; y++)
        {
            Console.WriteLine(lab[y]);
        }
    }

    static void print_box(Box box)
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }

        Console.SetCursorPosition(box.x, box.y);
        Console.ForegroundColor = ConsoleColor.Cyan;

        if (box.step < 10)
        {
            Console.Write(box.step);
        }
        else
        {
            Console.Write((char)(65 + box.step - 10));
        }
    }

    static void walk(Box box)
    {
        if (box.x < 0 || box.x >= size_x || box.y < 0 || box.y >= size_y)
        {
            return;
        }

        if (lab[box.y][box.x] != ' ')
        {
            return;
        }

        if (visited[box.x, box.y])
        {
            return;
        }

        visited[box.x, box.y] = true;
        boxes.Enqueue(box);
        print_box(box);
    }

    static int search()
    {
        visited = new bool[size_x, size_y];
        boxes = new Queue<Box>();

        start.step = 0;
        walk(start);

        while (boxes.Count > 0)
        {
            Box box = boxes.Dequeue();

            if (box.x == finish.x && box.y == finish.y)
            {
                boxes.Clear();
                return box.step;
            }

            box.step++;

            box.x--;
            walk(box);
            box.x++;

            box.x++;
            walk(box);
            box.x--;

            box.y--;
            walk(box);
            box.y++;

            box.y++;
            walk(box);
            box.y--;
        }

        return -1;
    }

    static void Main()
    {
        load();
        print();

        int length = search();

        Console.ResetColor();

        if (!Console.IsOutputRedirected)
        {
            Console.SetCursorPosition(0, size_y + 2);
        }
        else
        {
            Console.WriteLine();
        }

        Console.WriteLine(length);

        if (!Console.IsInputRedirected)
        {
            Console.ReadKey();
        }
    }
}
