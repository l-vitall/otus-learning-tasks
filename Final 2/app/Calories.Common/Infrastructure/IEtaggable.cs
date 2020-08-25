namespace Calories.Common.Infrastructure
{
    public interface IEtaggable
    {
        string GetEtag();
    }
}