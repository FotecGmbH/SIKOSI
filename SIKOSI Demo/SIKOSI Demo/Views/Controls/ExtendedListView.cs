// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.11.2020 07:43
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace SIKOSI.Sample02.Views.Controls
{
    public class ExtendedListView : ListView
    {
        public static readonly BindableProperty TappedCommandProperty =
            BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public static readonly BindableProperty ItemAppearingCommandProperty =
            BindableProperty.Create(nameof(ItemAppearingCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public static readonly BindableProperty ItemDisappearingCommandProperty =
            BindableProperty.Create(nameof(ItemDisappearingCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public ExtendedListView() : this(ListViewCachingStrategy.RecycleElement)
        {
        }

        public ExtendedListView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
            ItemSelected += OnItemSelected;
            ItemTapped += OnItemTapped;
            ItemAppearing += OnItemAppearing;
            ItemDisappearing += OnItemDisappering;
        }

        #region Properties

        public ICommand TappedCommand
        {
            get { return (ICommand) GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }

        public ICommand ItemAppearingCommand
        {
            get { return (ICommand) GetValue(ItemAppearingCommandProperty); }
            set { SetValue(ItemAppearingCommandProperty, value); }
        }


        public ICommand ItemDisappearingCommand
        {
            get { return (ICommand) GetValue(ItemDisappearingCommandProperty); }
            set { SetValue(ItemDisappearingCommandProperty, value); }
        }

        #endregion


        public void ScrollToFirst()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (ItemsSource != null && ItemsSource.Cast<object>().Count() > 0)
                    {
                        var msg = ItemsSource.Cast<object>().FirstOrDefault();
                        if (msg != null) ScrollTo(msg, ScrollToPosition.Start, false);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            });
        }

        public void ScrollToLast()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (ItemsSource != null && ItemsSource.Cast<object>().Count() > 0)
                    {
                        var msg = ItemsSource.Cast<object>().LastOrDefault();
                        if (msg != null) ScrollTo(msg, ScrollToPosition.End, false);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            });
        }


        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = (ExtendedListView) sender;
            if (e == null) return;
            listView.SelectedItem = null;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (TappedCommand != null) TappedCommand?.Execute(e.Item);
            SelectedItem = null;
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ItemAppearingCommand != null) ItemAppearingCommand?.Execute(e.Item);
        }


        private void OnItemDisappering(object sender, ItemVisibilityEventArgs e)
        {
            ItemDisappearingCommand?.Execute(e.Item);
        }
    }
}