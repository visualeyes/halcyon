namespace Halcyon.HAL.Filters
{
    public interface IHalModelAttribute
    {
        string LinkBase { get; }
        bool ForceHal { get; }
    }
}
