using Shared.Models;
using System.Net.Http.Json;
using Client.Static;

namespace Client.Services;

internal sealed class InMemoryDataCach
{
    private readonly HttpClient _httpClient;

    public InMemoryDataCach(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private List<Category> _categories = null;

    public List<Category> Categories
    {
        get => _categories;
        set
        {
            _categories = value;
            NotifyCategoriesDataChanged();
        }
    }

    private bool _gettingCategoriesFromDatabaseAndCaching = false;
    
    internal async Task GetCategoriesFromDatabaseAndCache()
    {
        if (_gettingCategoriesFromDatabaseAndCaching == false)
        {
            // only allow one get request at the time 
            _gettingCategoriesFromDatabaseAndCaching = true;
            Categories = await _httpClient.GetFromJsonAsync<List<Category>>(ApiEndPoints.SCategories);
            _gettingCategoriesFromDatabaseAndCaching = false;
        }
        
    }

    internal event Action OnCategoriesDataChanged;
    private void NotifyCategoriesDataChanged() => OnCategoriesDataChanged?.Invoke();
}