﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TravelApp_G15.Models;
using TravelApp_G15.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TravelApp_G15.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TripDetail : Page
    {
        private ICollection<Item> items;
        private ICollection<Category> categories;
        private ItemViewModel itemViewModel;
        private CategoryViewModel catViewModel;

        public TripDetail()
        {
            this.InitializeComponent();

            items = new List<Item>();
            categories = new List<Category>();
            itemViewModel = new ItemViewModel();
            catViewModel = new CategoryViewModel();

            GetAllItems(itemViewModel);
            GetAllCategories(catViewModel);
        }

        private async void GetAllItems(ItemViewModel viewModel)
        {
            ApplicationDataContainer local = ApplicationData.Current.LocalSettings;
            int tripID = Int32.Parse(local.Values["tripID"].ToString());

           // int categoryID = Int32.Parse(local.Values["categoryID"].ToString());
            await viewModel.GetAllItems(tripID);
           // await viewModel.GetItemsByCategorie(tripID, categoryID);
            items = viewModel.Items;
            ItemList.ItemsSource = items;
        }

        private async void GetAllCategories(CategoryViewModel viewModel)
        {
            ApplicationDataContainer local = ApplicationData.Current.LocalSettings;
            int tripID = Int32.Parse(local.Values["tripID"].ToString());
            await viewModel.GetAllCategories(tripID);

            categories = viewModel.Categories;
            CatList.ItemsSource = categories;
        }

        #region Navigation
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("vacations", typeof(Dashboard)),
            ("items", typeof(TripDetail)),
            ("tasks", typeof(Views.Task))
        };

        private void Navigation_PaneOpened(NavigationView sender, object args)
        {
            Menu.Content = "MENU";
        }

        private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                this.Frame.Navigate(typeof(Login));
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(Login);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = this.Frame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                this.Frame.Navigate(_page, null, transitionInfo);
            }
        }
        private void Navigation_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = (NavigationViewItem)Navigation.SettingsItem;
            settings.Content = "Logout";
            settings.FontSize = 20;
            settings.Icon = new SymbolIcon((Symbol)0xE106);
        }
        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (this.Frame.CanGoBack) this.Frame.GoBack();
        }
        #endregion
    }
}
