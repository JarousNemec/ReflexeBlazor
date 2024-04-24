using System.Reflection;

namespace ReflexeBlazor.Services;

public class FilterService
{
    private readonly List<BindingFlags> _filterFlags;
    public List<BindingFlags> FilterFlags => _filterFlags;

    public delegate void FilterChange();

    public event FilterChange OnFilterChange;

    public FilterService()
    {
        _filterFlags = new List<BindingFlags>();
    }

    public void ChangeFilter(BindingFlags flag)
    {
        if (!_filterFlags.Contains(flag))
        {
            _filterFlags.Add(flag);
        }
        else
        {
            _filterFlags.Remove(flag);
        }
        OnFilterChange?.Invoke();
    }
}