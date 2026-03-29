namespace SingleNeuronConsole;

public class Neuron
{
    private const decimal InitialWeight = 0.5m;
    private const decimal ZeroValue = 0m;

    public decimal weight { get; private set; } = InitialWeight;
    public decimal lastError { get; private set; }
    public decimal Smoothing { get; set; } = 0.02m;
    public decimal Tolerance { get; set; } = 0.000001m;
    public decimal actualRes { get; private set; }

    public decimal ProcInput(decimal input) => input * weight;

    public decimal ProcOutput(decimal output)
    {
        EnsureReverseConversionIsAvailable();
        return output / weight;
    }

    public void Learn(decimal input, decimal expectedRes)
    {
        UpdateState(input, expectedRes);

        if (input == ZeroValue)
        {
            return;
        }

        weight = GetAdjustedWeight(input);
        UpdateState(input, expectedRes);
    }

    private decimal GetAdjustedWeight(decimal input)
    {
        decimal learningStep = (lastError / input) * Smoothing;
        return weight + learningStep;
    }

    private void UpdateState(decimal input, decimal expectedRes)
    {
        actualRes = ProcInput(input);
        lastError = expectedRes - actualRes;
    }

    private void EnsureReverseConversionIsAvailable()
    {
        if (weight == ZeroValue)
        {
            throw new InvalidOperationException("Reverse conversion is not possible when weight is zero.");
        }
    }
}
