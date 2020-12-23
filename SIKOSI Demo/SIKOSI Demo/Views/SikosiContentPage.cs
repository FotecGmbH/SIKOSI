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

using Microsoft.Extensions.DependencyInjection;
using SIKOSI.Sample02.ViewModels;
using Xamarin.Forms;

namespace SIKOSI.Sample02.Views
{
    public class SikosiContentPage<T> : ContentPage where T : ViewModelBase
    {
        public SikosiContentPage()
        {
            BindingContext = ViewModel;
            Init();
        }

        #region Properties

        public T ViewModel { get; set; } = App.ServiceProvider.GetService<T>();

        #endregion

        public async void Init()
        {
            try
            {
                await ViewModel.OnInitialized();
            }
            catch
            {
                //await Navigation.PushAsync(new NavigationPage(new ViewLogin()));
            }
        }
    }
}