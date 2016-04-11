namespace Halcyon.HAL.Filters
{
    public interface IHalLinkAttribute
    {
        string Rel { get; }
        string Href { get; }
        string Title { get; }
        string Method { get; }
    }
}