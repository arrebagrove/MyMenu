﻿using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace MyMenu
{
	public class FoodListPageViewModel : MyMenuBaseListViewModel<FoodViewModel, Food>
    {
		public FoodListPageViewModel ()
		{
			Title = "Home";
			Items = new ObservableCollection<FoodViewModel> ();
            RefreshCommand.Execute(null);
            CartViewModel = new XCartViewModel();

        }

        private XCartViewModel cartViewModel;

        public XCartViewModel CartViewModel
        {
            get { return cartViewModel; }
            set { cartViewModel = value; RaisePropertyChanged(); }
        }


        public FavoriteManager FavoriteManager { get; } = DependencyService.Get<IAzureDataManager<FavoriteItem>>() as FavoriteManager;

	    public override Task SyncDataAsync()
	    {
	        return base.SyncDataAsync().ContinueWith(async (x)=>
             await FavoriteManager.SyncAsync(Settings.CurrentUserId));
	    }

	    public override async Task LoadItems()
		{
			var items = await DataManager?.GetAsync();
			var favorites = await FavoriteManager?.GetAsync();

			var fooditems = from fi in items
				            join  fav in favorites on fi.Id equals fav.FoodItemId into prodGroup
				            from g in prodGroup.DefaultIfEmpty (null)
				            select new {FoodItem = fi, FavoriteItem = g};

			Items.Clear ();

			foreach (var item in fooditems) {
				item.FoodItem.IsFavorite = (item.FavoriteItem != null);
				Items.Add (new FoodViewModel (item.FoodItem));
			}
	        new XCartViewModel();
		}
        
    }
}