using System;
using System.Collections.Generic;

internal readonly record struct Box(int X, int Y, int Step);

internal static class Program
{
    private static readonly string[] Labyrinth =
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

    private static readonly (int Dx, int Dy)[] Directions =
    {
        (-1, 0),
        (1, 0),
        (0, -1),
        (0, 1)
    };

    private static bool[,] _visited = null!;
    private static Queue<Box> _frontier = null!;
    private static int _width;
    private static int _height;
    private static Box _start;
    private static Box _finish;

    private static void Main()
    {
        InitializeMazeInfo();
        PrintLabyrinth();

        int pathLength = SearchShortestPath();

        Console.ResetColor();

        if (!Console.IsOutputRedirected)
        {
            Console.SetCursorPosition(0, _height + 2);
        }
        else
        {
            Console.WriteLine();
        }

        Console.WriteLine(pathLength);

        if (!Console.IsInputRedirected)
        {
            Console.ReadKey();
        }
    }

    private static void InitializeMazeInfo()
    {
        _width = Labyrinth[0].Length;
        _height = Labyrinth.Length;
        _start = new Box(0, 1, 0);
        _finish = new Box(_width - 1, _height - 2, 0);
    }

    private static void PrintLabyrinth()
    {
        if (!Console.IsOutputRedirected)
        {
            Console.Clear();
        }

        for (int y = 0; y < _height; y++)
        {
            Console.WriteLine(Labyrinth[y]);
        }
    }

    private static int SearchShortestPath()
    {
        _visited = new bool[_width, _height];
        _frontier = new Queue<Box>();

        TryVisit(_start);

        while (_frontier.Count > 0)
        {
            Box current = _frontier.Dequeue();

            if (IsFinish(current))
            {
                _frontier.Clear();
                return current.Step;
            }

            EnqueueNeighbors(current);
        }

        return -1;
    }

    private static void EnqueueNeighbors(Box current)
    {
        int nextStep = current.Step + 1;

        foreach (var direction in Directions)
        {
            var next = new Box(current.X + direction.Dx, current.Y + direction.Dy, nextStep);
            TryVisit(next);
        }
    }

    private static void TryVisit(Box candidate)
    {
        if (!IsInside(candidate.X, candidate.Y))
        {
            return;
        }

        if (Labyrinth[candidate.Y][candidate.X] != ' ')
        {
            return;
        }

        if (_visited[candidate.X, candidate.Y])
        {
            return;
        }

        _visited[candidate.X, candidate.Y] = true;
        _frontier.Enqueue(candidate);
        PrintBox(candidate);
    }

    private static void PrintBox(Box box)
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }

        Console.SetCursorPosition(box.X, box.Y);
        Console.ForegroundColor = ConsoleColor.Cyan;

        if (box.Step < 10)
        {
            Console.Write(box.Step);
            return;
        }

        Console.Write((char)(65 + box.Step - 10));
    }

    private static bool IsFinish(Box box) => box.X == _finish.X && box.Y == _finish.Y;

    private static bool IsInside(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;
}
