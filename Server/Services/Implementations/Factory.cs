using WebTemplate.Enums;
using WebTemplate.Services.Interfaces;

namespace WebTemplate.Services.Implementations;

public class Factory : IFactory
{
    private readonly IAlgorithm _railfence;

    public Factory(IAlgorithm railfence)
    {
        _railfence = railfence;
    }

    public IAlgorithm GetService(AlgorithmType type)
    {
        return type switch
        {
            AlgorithmType.Railfence => _railfence,
            // Add other algorithms here as needed
            _ => throw new NotImplementedException($"Service for {type} is not implemented.")
        };
    }
}
