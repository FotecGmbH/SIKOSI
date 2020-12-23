// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 16.12.2020 15:42
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Linq;
using SIKOSI.Sample02.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SIKOSI.Sample02.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewLogin : SikosiContentPage<ViewModelLogin>
    {
        public ViewLogin()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            ViewModel.LoginSuccessful -= ViewModel_LoginSuccessful;
            ViewModel.LoginNotSuccessful -= ViewModel_LoginNotSuccessful;
            ViewModel.LoginSuccessful += ViewModel_LoginSuccessful;
            ViewModel.LoginNotSuccessful += ViewModel_LoginNotSuccessful;
        }

        private async void ViewModel_LoginNotSuccessful(object sender, EventArgs e)
        {
            await DisplayAlert("Login failed!", "Please Try again.", "OK");
        }

        private async void ViewModel_LoginSuccessful(object sender, EventArgs e)
        {
            await DisplayAlert("Login successful!", string.Empty, "OK");
            await Navigation.PushAsync(new NavigationPage(new AppShell()));
        }

        private async void SendToRegistration(object sender, EventArgs e)
        {
            if (Navigation.ModalStack.Any()) await Navigation.PopModalAsync();

            await Navigation.PushModalAsync(new NavigationPage(new ViewRegistration()));
        }
    }
}