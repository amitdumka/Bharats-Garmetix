using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SQLite;

namespace PointOfSale.Pages
{
    public class ProductListPage : ContentPage
    {
        private readonly string dbPath;
        private readonly ObservableCollection<Bharat.Inventory.Product> products = new();
        private readonly ListView listView;

        public ProductListPage(string databasePath)
        {
            dbPath = databasePath;
            Title = "Products";

            var addButton = new ToolbarItem { Text = "Add", Priority = 0 };
            addButton.Clicked += async (s, e) => await OnAddClicked();
            ToolbarItems.Add(addButton);

            listView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = products,
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid { Padding = 10 };
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    var name = new Label { FontAttributes = FontAttributes.Bold };
                    name.SetBinding(Label.TextProperty, "Name");

                    var mrp = new Label { HorizontalOptions = LayoutOptions.End };
                    mrp.SetBinding(Label.TextProperty, new Binding("MRP", BindingMode.OneWay, null, null, "{0:C}"));

                    grid.Add(name, 0, 0);
                    grid.Add(mrp, 1, 0);

                    return new ViewCell { View = grid };
                })
            };

            listView.ItemTapped += async (s, e) =>
            {
                if (e.Item is Bharat.Inventory.Product p)
                {
                    await OpenEdit(p);
                }
                ((ListView)s).SelectedItem = null;
            };

            Content = new StackLayout
            {
                Children = { listView }
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            var db = new SQLiteAsyncConnection(dbPath);
            await db.CreateTableAsync<Bharat.Inventory.Product>();
            var list = await db.Table<Bharat.Inventory.Product>().ToListAsync();

            products.Clear();
            foreach (var p in list)
                products.Add(p);
        }

        private async Task OnAddClicked()
        {
            var product = new Bharat.Inventory.Product();
            var page = PointOfSale.Helpers.FormGenerator.BuildPage(product, "New Product", async p =>
            {
                var db = new SQLiteAsyncConnection(dbPath);
                await db.CreateTableAsync<Bharat.Inventory.Product>();
                await db.InsertOrReplaceAsync(p);
            });

            await Navigation.PushAsync(page);
        }

        private async Task OpenEdit(Bharat.Inventory.Product product)
        {
            // Build page bound to existing product instance
            var page = PointOfSale.Helpers.FormGenerator.BuildPage(product, "Edit Product", async p =>
            {
                var db = new SQLiteAsyncConnection(dbPath);
                await db.CreateTableAsync<Bharat.Inventory.Product>();
                await db.InsertOrReplaceAsync(p);
            });

            await Navigation.PushAsync(page);
        }
    }
}
