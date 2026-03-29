using System.Diagnostics;
using System.Text;

internal static class Program
{
    private const int MapWidth = 170;
    private const int MapHeight = 49;
    private const int FillDelayMilliseconds = 0;
    private const int WallColor = 1;
    private const int FillColor = 2;
    private const int EnterRightColor = 3;
    private const int EnterDownColor = 4;
    private const int EnterLeftColor = 5;
    private const int EnterUpColor = 6;
    private const int SafeZoneRadius = 2;

    private static readonly Random Randomizer = new(12145);
    private static readonly int[,] Map = new int[MapWidth, MapHeight];
    private static readonly string[] Symbols =
    {
        " ",
        " ",
        " ",
        ">",
        "v",
        "<",
        "^"
    };

    private static readonly ConsoleColor[] ForegroundColors =
    {
        ConsoleColor.Black,
        ConsoleColor.DarkGray,
        ConsoleColor.White,
        ConsoleColor.Red,
        ConsoleColor.Green,
        ConsoleColor.Cyan,
        ConsoleColor.Yellow
    };

    private static readonly ConsoleColor[] BackgroundColors =
    {
        ConsoleColor.Black,
        ConsoleColor.DarkGray,
        ConsoleColor.DarkBlue,
        ConsoleColor.Black,
        ConsoleColor.Black,
        ConsoleColor.Black,
        ConsoleColor.Black
    };

    private static int _filledCells;

    private static void Main()
    {
        try
        {
            PrepareConsole();
            Console.Clear();

            InitializeMap();

            var start = GetStartPoint();
            HighlightStart(start);
            WaitForOptionalKeyPress();
            ClearStartMarker(start);

            Stopwatch stopwatch = Stopwatch.StartNew();
            FloodFill(start.X, start.Y);
            stopwatch.Stop();

            PrintSummary(stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            RestoreConsole();
        }
    }

    private static (int X, int Y) GetStartPoint() => (MapWidth / 2, MapHeight / 2);

    private static void InitializeMap()
    {
        _filledCells = 0;

        ClearMap();
        DrawFrame();
        PlaceHorizontalWalls();
        PlaceVerticalWalls();
        PlaceRectangles();
        ClearSafeZone();
        RedrawMap();
    }

    private static void ClearMap()
    {
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                Map[x, y] = 0;
            }
        }
    }

    private static void DrawFrame()
    {
        for (int x = 0; x < MapWidth; x++)
        {
            Map[x, 0] = WallColor;
            Map[x, MapHeight - 1] = WallColor;
        }

        for (int y = 0; y < MapHeight; y++)
        {
            Map[0, y] = WallColor;
            Map[MapWidth - 1, y] = WallColor;
        }
    }

    private static void PlaceHorizontalWalls()
    {
        for (int i = 0; i < 90; i++)
        {
            int x = Randomizer.Next(1, MapWidth - 2);
            int y = Randomizer.Next(1, MapHeight - 1);
            int length = Randomizer.Next(4, 13);

            for (int dx = 0; dx < length && x + dx < MapWidth - 1; dx++)
            {
                TryPlaceWall(x + dx, y);
            }
        }
    }

    private static void PlaceVerticalWalls()
    {
        for (int i = 0; i < 70; i++)
        {
            int x = Randomizer.Next(1, MapWidth - 1);
            int y = Randomizer.Next(1, MapHeight - 2);
            int length = Randomizer.Next(3, 10);

            for (int dy = 0; dy < length && y + dy < MapHeight - 1; dy++)
            {
                TryPlaceWall(x, y + dy);
            }
        }
    }

    private static void PlaceRectangles()
    {
        for (int i = 0; i < 18; i++)
        {
            int x = Randomizer.Next(1, MapWidth - 6);
            int y = Randomizer.Next(1, MapHeight - 4);
            int width = Randomizer.Next(2, 7);
            int height = Randomizer.Next(2, 5);

            for (int dy = 0; dy < height && y + dy < MapHeight - 1; dy++)
            {
                for (int dx = 0; dx < width && x + dx < MapWidth - 1; dx++)
                {
                    TryPlaceWall(x + dx, y + dy);
                }
            }
        }
    }

    private static void TryPlaceWall(int x, int y)
    {
        if (!IsInsideSafeZone(x, y))
        {
            Map[x, y] = WallColor;
        }
    }

    private static void ClearSafeZone()
    {
        var start = GetStartPoint();

        for (int y = start.Y - SafeZoneRadius; y <= start.Y + SafeZoneRadius; y++)
        {
            for (int x = start.X - SafeZoneRadius; x <= start.X + SafeZoneRadius; x++)
            {
                if (IsInsideMap(x, y))
                {
                    Map[x, y] = 0;
                }
            }
        }
    }

    private static void RedrawMap()
    {
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                DrawCell(x, y, Map[x, y]);
            }
        }
    }

    private static void HighlightStart((int X, int Y) start) => DrawCell(start.X, start.Y, EnterRightColor);

    private static void ClearStartMarker((int X, int Y) start) => DrawCell(start.X, start.Y, 0);

    private static void DrawCell(int x, int y, int color)
    {
        if (!IsInsideMap(x, y))
        {
            return;
        }

        Map[x, y] = color;
        Console.ForegroundColor = ForegroundColors[color];
        Console.BackgroundColor = BackgroundColors[color];
        Console.SetCursorPosition(x, y);
        Console.Write(Symbols[color]);
    }

    private static void FloodFill(int x, int y)
    {
        if (!CanFill(x, y))
        {
            return;
        }

        DrawCell(x, y, EnterRightColor);
        FloodFill(x + 1, y);

        DrawCell(x, y, EnterDownColor);
        FloodFill(x, y + 1);

        DrawCell(x, y, EnterLeftColor);
        FloodFill(x - 1, y);

        DrawCell(x, y, EnterUpColor);
        FloodFill(x, y - 1);

        DrawCell(x, y, FillColor);
        Thread.Sleep(FillDelayMilliseconds);
        _filledCells++;
    }

    private static bool CanFill(int x, int y)
    {
        if (!IsInsideMap(x, y))
        {
            return false;
        }

        return Map[x, y] == 0;
    }

    private static bool IsInsideMap(int x, int y) => x >= 0 && x < MapWidth && y >= 0 && y < MapHeight;

    private static bool IsInsideSafeZone(int x, int y)
    {
        var start = GetStartPoint();
        return Math.Abs(x - start.X) <= SafeZoneRadius && Math.Abs(y - start.Y) <= SafeZoneRadius;
    }

    private static void WaitForOptionalKeyPress()
    {
        Console.ResetColor();
        Console.SetCursorPosition(0, MapHeight + 1);
        Console.Write("Нажми любую клавишу...");

        if (!Console.IsInputRedirected)
        {
            Console.ReadKey(true);
        }
    }

    private static void PrintSummary(long elapsedMilliseconds)
    {
        Console.ResetColor();
        Console.SetCursorPosition(0, MapHeight + 1);
        Console.Write(new string(' ', 40));
        Console.SetCursorPosition(0, MapHeight + 1);
        Console.WriteLine($"Время: {elapsedMilliseconds} ms");
        Console.WriteLine($"Залито ячеек: {_filledCells}");
    }

    private static void RestoreConsole()
    {
        Console.ResetColor();
        Console.CursorVisible = true;
        Console.SetCursorPosition(0, MapHeight + 3);
    }

    private static void PrepareConsole()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            int wantedWidth = Math.Min(MapWidth + 1, Console.LargestWindowWidth);
            int wantedHeight = Math.Min(MapHeight + 4, Console.LargestWindowHeight);

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
