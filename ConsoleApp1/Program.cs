using System.Diagnostics;
using System.Text;

internal static class Program
{
    static int maxx = 170;
    static int maxy = 49;
    static int count = 0;
    static Random rnd = new Random(12145);

    static int[,] map = new int[maxx, maxy];

    static string[] symbols = new string[]
    {
        " ",   // 0 - empty
        " ",   // 1 - wall (solid color)
        " ",   // 2 - filled (solid color)
        ">",   // 3 - right
        "v",   // 4 - down
        "<",   // 5 - left
        "^"    // 6 - up
    };

    static ConsoleColor[] fore = new ConsoleColor[]
    {
        ConsoleColor.Black,      // 0 - empty
        ConsoleColor.DarkGray,   // 1 - wall
        ConsoleColor.White,      // 2 - filled
        ConsoleColor.Red,        // 3 - right
        ConsoleColor.Green,      // 4 - down
        ConsoleColor.Cyan,       // 5 - left
        ConsoleColor.Yellow      // 6 - up
    };

    static ConsoleColor[] back = new ConsoleColor[]
    {
        ConsoleColor.Black,      // 0 - empty
        ConsoleColor.DarkGray,   // 1 - wall
        ConsoleColor.DarkBlue,   // 2 - filled
        ConsoleColor.Black,      // 3 - right
        ConsoleColor.Black,      // 4 - down
        ConsoleColor.Black,      // 5 - left
        ConsoleColor.Black       // 6 - up
    };

    static int fillDelayMs = 0;

    static void Main()
    {
        try
        {
            PrepareConsole();
            Console.Clear();

            init();

            int startX = maxx / 2;
            int startY = maxy / 2;

            show(startX, startY, 3);

            Console.ResetColor();
            Console.SetCursorPosition(0, maxy + 1);
            Console.Write("Press any key to start fill...");

            if (!Console.IsInputRedirected)
            {
                Console.ReadKey(true);
            }

            show(startX, startY, 0);

            Console.SetCursorPosition(0, maxy + 1);
            Console.Write(new string(' ', 40));

            Stopwatch sw = Stopwatch.StartNew();
            Fill(startX, startY);
            sw.Stop();

            Console.ResetColor();
            Console.SetCursorPosition(0, maxy + 1);
            Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"Filled cells: {count}");
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, maxy + 3);
        }
    }

    static void show(int x, int y, int color)
    {
        if (!Inside(x, y))
        {
            return;
        }

        map[x, y] = color;
        Console.ForegroundColor = fore[color];
        Console.BackgroundColor = back[color];
        Console.SetCursorPosition(x, y);
        Console.Write(symbols[color]);
    }

    static void init()
    {
        count = 0;

        for (int y = 0; y < maxy; y++)
        {
            for (int x = 0; x < maxx; x++)
            {
                map[x, y] = 0;
            }
        }

        for (int x = 0; x < maxx; x++)
        {
            map[x, 0] = 1;
            map[x, maxy - 1] = 1;
        }

        for (int y = 0; y < maxy; y++)
        {
            map[0, y] = 1;
            map[maxx - 1, y] = 1;
        }

        for (int i = 0; i < 90; i++)
        {
            int x = rnd.Next(1, maxx - 2);
            int y = rnd.Next(1, maxy - 1);
            int len = rnd.Next(4, 13);

            for (int dx = 0; dx < len && x + dx < maxx - 1; dx++)
            {
                if (!InsideSafeZone(x + dx, y))
                {
                    map[x + dx, y] = 1;
                }
            }
        }

        for (int i = 0; i < 70; i++)
        {
            int x = rnd.Next(1, maxx - 1);
            int y = rnd.Next(1, maxy - 2);
            int len = rnd.Next(3, 10);

            for (int dy = 0; dy < len && y + dy < maxy - 1; dy++)
            {
                if (!InsideSafeZone(x, y + dy))
                {
                    map[x, y + dy] = 1;
                }
            }
        }

        for (int i = 0; i < 18; i++)
        {
            int x = rnd.Next(1, maxx - 6);
            int y = rnd.Next(1, maxy - 4);
            int width = rnd.Next(2, 7);
            int height = rnd.Next(2, 5);

            for (int dy = 0; dy < height && y + dy < maxy - 1; dy++)
            {
                for (int dx = 0; dx < width && x + dx < maxx - 1; dx++)
                {
                    if (!InsideSafeZone(x + dx, y + dy))
                    {
                        map[x + dx, y + dy] = 1;
                    }
                }
            }
        }

        for (int y = maxy / 2 - 2; y <= maxy / 2 + 2; y++)
        {
            for (int x = maxx / 2 - 2; x <= maxx / 2 + 2; x++)
            {
                if (Inside(x, y))
                {
                    map[x, y] = 0;
                }
            }
        }

        for (int y = 0; y < maxy; y++)
        {
            for (int x = 0; x < maxx; x++)
            {
                show(x, y, map[x, y]);
            }
        }
    }

    static void Fill(int x, int y)
    {
        if (!Inside(x, y))
        {
            return;
        }

        if (map[x, y] > 0)
        {
            return;
        }

        show(x, y, 3);
        Fill(x + 1, y);

        show(x, y, 4);
        Fill(x, y + 1);

        show(x, y, 5);
        Fill(x - 1, y);

        show(x, y, 6);
        Fill(x, y - 1);

        show(x, y, 2);
        Thread.Sleep(fillDelayMs);
        count++;
    }

    static bool Inside(int x, int y)
    {
        return x >= 0 && x < maxx && y >= 0 && y < maxy;
    }

    static bool InsideSafeZone(int x, int y)
    {
        return Math.Abs(x - maxx / 2) <= 2 && Math.Abs(y - maxy / 2) <= 2;
    }

    static void PrepareConsole()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Title = "Recursive Flood Fill";

        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            int wantedWidth = Math.Min(maxx + 1, Console.LargestWindowWidth);
            int wantedHeight = Math.Min(maxy + 4, Console.LargestWindowHeight);

            if (Console.BufferWidth < wantedWidth || Console.BufferHeight < wantedHeight)
            {
                int bufferWidth = Math.Max(Console.BufferWidth, wantedWidth);
                int bufferHeight = Math.Max(Console.BufferHeight, wantedHeight);
                Console.SetBufferSize(bufferWidth, bufferHeight);
            }

            if (Console.WindowWidth < wantedWidth || Console.WindowHeight < wantedHeight)
            {
                Console.SetWindowSize(wantedWidth, wantedHeight);
            }
        }
        catch
        {
        }
    }
}
