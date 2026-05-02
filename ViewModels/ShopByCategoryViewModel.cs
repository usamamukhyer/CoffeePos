using System.Collections.ObjectModel;
using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class ShopByCategoryViewModel : BaseViewModel
{
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
    private readonly LocalMenuCacheService _cache = new();
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public ObservableCollection<CategoryModel> Categories { get; } = new();
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public bool IsEmpty => !IsBusy && !HasError && Categories.Count == 0;

    public async Task LoadAsync()
    {
        if (IsBusy || Categories.Count > 0)
            return;

        var cachedCategories = await _cache.GetCategoriesAsync();
        ReplaceCategories(cachedCategories);
        _ = SyncCategoriesAsync();
        if (cachedCategories.Count > 0)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(HasError));

            var categories = await _apiClient.GetAsync<IReadOnlyList<ApiCategory>>("api/categories") ?? [];
            await _cache.UpsertCategoriesAsync(categories);
            ReplaceCategories(await _cache.GetCategoriesAsync());
            OnPropertyChanged(nameof(IsEmpty));
        }
        catch (Exception ex) when (ex is HttpRequestException or ApiException)
        {
            ErrorMessage = "No se pudo cargar el menu. Intentalo de nuevo.";
            OnPropertyChanged(nameof(HasError));
            Categories.Clear();
            OnPropertyChanged(nameof(IsEmpty));
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    public ICommand SelectCategoryCommand => new RelayCommand(async value =>
    {
        if (value is CategoryModel category)
        {
            OrderSessionService.Instance.SelectedCategory = category;
            await Shell.Current.GoToAsync(nameof(Views.OurMenuPage));
        }
    });

    private async Task SyncCategoriesAsync()
    {
        try
        {
            var categories = await _apiClient.GetAsync<IReadOnlyList<ApiCategory>>("api/categories") ?? [];
            await _cache.UpsertCategoriesAsync(categories);
        }
        catch (Exception ex) when (ex is HttpRequestException or ApiException)
        {
        }
    }

    private void ReplaceCategories(IEnumerable<CategoryModel> categories)
    {
        Categories.Clear();
        foreach (var category in categories)
            Categories.Add(category);
        OnPropertyChanged(nameof(IsEmpty));
    }

    private static string GetCategoryImage(string name)
    {
        if (name.Contains("tea", StringComparison.OrdinalIgnoreCase))
            return "coffee_tea.png";
        if (name.Contains("frappe", StringComparison.OrdinalIgnoreCase))
            return "coffee_frappe.png";
        if (name.Contains("chocolate", StringComparison.OrdinalIgnoreCase))
            return "coffee_chocolate.png";

        return "coffee_cup.png";
    }
}
