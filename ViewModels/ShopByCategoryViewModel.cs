using System.Collections.ObjectModel;
using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class ShopByCategoryViewModel : BaseViewModel
{
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
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

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(HasError));

            var categories = await _apiClient.GetAsync<IReadOnlyList<ApiCategory>>("api/categories") ?? [];
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(new CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Key = category.Key,
                    Image = GetCategoryImage(category.Name)
                });
            }
            OnPropertyChanged(nameof(IsEmpty));
        }
        catch (Exception ex) when (ex is HttpRequestException or ApiException)
        {
            ErrorMessage = "Unable to load menu. Please try again.";
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
