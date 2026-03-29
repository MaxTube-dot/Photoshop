namespace SingleNeuronConsole;

internal class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        decimal km = 1;
        decimal miles = 0.621371m;
        Neuron neuron = new Neuron();

        int iteration = 0;
        int maxIterations = 10000;

        Console.WriteLine();

        do
        {
            iteration++;
            neuron.Learn(km, miles);

            Console.WriteLine(
                $"Итерация {iteration,3}: ошибка = {neuron.lastError:F6}; " +
                $"текущий результат = {neuron.actualRes:F6}; вес = {neuron.weight:F6}");
        }
        while (Math.Abs(neuron.lastError) >= neuron.Tolerance && iteration < maxIterations);

        Console.WriteLine();
        if (iteration >= maxIterations && Math.Abs(neuron.lastError) >= neuron.Tolerance)
        {
            Console.WriteLine("Достигнут лимит итераций.");
        }

        Console.WriteLine("Обучение завершено");
        Console.WriteLine($"100 км = {neuron.ProcInput(100m):F6} миль");
        Console.WriteLine($"515 км = {neuron.ProcInput(515m):F6} миль");
        Console.WriteLine($"10 миль = {neuron.ProcOutput(10m):F6} км");
    }
}
