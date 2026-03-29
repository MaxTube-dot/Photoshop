namespace SingleNeuronConsole;

internal class Program
{
    private const decimal TrainingKilometers = 1m;
    private const decimal TargetMiles = 0.621371m;
    private const int MaxIterations = 10000;

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Neuron neuron = new();

        Console.WriteLine();
        int iteration = RunTraining(neuron);

        Console.WriteLine();
        PrintLimitMessageIfNeeded(neuron, iteration);
        PrintSummary(neuron);
    }

    private static int RunTraining(Neuron neuron)
    {
        int iteration = 0;

        while (iteration < MaxIterations)
        {
            iteration++;
            neuron.Learn(TrainingKilometers, TargetMiles);
            PrintIteration(neuron, iteration);

            if (Math.Abs(neuron.lastError) < neuron.Tolerance)
            {
                break;
            }
        }

        return iteration;
    }

    private static void PrintIteration(Neuron neuron, int iteration)
    {
        Console.WriteLine(
            $"Итерация {iteration,3}: ошибка = {neuron.lastError:F6}; " +
            $"текущий результат = {neuron.actualRes:F6}; вес = {neuron.weight:F6}");
    }

    private static void PrintLimitMessageIfNeeded(Neuron neuron, int iteration)
    {
        if (iteration >= MaxIterations && Math.Abs(neuron.lastError) >= neuron.Tolerance)
        {
            Console.WriteLine("Достигнут лимит итераций.");
        }
    }

    private static void PrintSummary(Neuron neuron)
    {
        Console.WriteLine("Обучение завершено");
        PrintConversion("100 км", neuron.ProcInput(100m), "миль");
        PrintConversion("515 км", neuron.ProcInput(515m), "миль");
        PrintConversion("10 миль", neuron.ProcOutput(10m), "км");
    }

    private static void PrintConversion(string sourceValue, decimal convertedValue, string targetUnit)
    {
        Console.WriteLine($"{sourceValue} = {convertedValue:F6} {targetUnit}");
    }
}
