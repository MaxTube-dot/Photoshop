namespace SingleNeuronConsole;

public class Neuron
{
    // Simplified educational example of one neuron with one weight.
    public decimal weight = 0.5m;
    public decimal lastError { get; private set; }
    public decimal Smoothing { get; set; } = 0.02m;
    public decimal Tolerance { get; set; } = 0.000001m;
    public decimal actualRes;

    public decimal ProcInput(decimal input)
    {
        return input * weight;
    }

    public decimal ProcOutput(decimal output)
    {
        if (weight == 0m)
        {
            throw new InvalidOperationException("Reverse conversion is not possible when weight is zero.");
        }

        return output / weight;
    }

    public void Learn(decimal input, decimal expectedRes)
    {
        actualRes = ProcInput(input);
        lastError = expectedRes - actualRes;

        if (input == 0m)
        {
            return;
        }

        // Use Smoothing as a learning rate so the weight changes gradually
        // and the convergence can be seen over multiple iterations.
        decimal correction = (lastError / input) * Smoothing;
        weight += correction;

        actualRes = ProcInput(input);
        lastError = expectedRes - actualRes;
    }
}
