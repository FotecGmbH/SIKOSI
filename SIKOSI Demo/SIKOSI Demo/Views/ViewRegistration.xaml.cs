// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 21.12.2020 09:58
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
    public partial class ViewRegistration : SikosiContentPage<ViewModelRegistration>
    {
        public ViewRegistration()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            ViewModel.RegistrationSuccessful -= ViewModel_RegistrationSuccessful;
            ViewModel.RegistrationNotSuccessful -= ViewModel_RegistrationNotSuccessful;
            ViewModel.RegistrationSuccessful += ViewModel_RegistrationSuccessful;
            ViewModel.RegistrationNotSuccessful += ViewModel_RegistrationNotSuccessful;
        }

        private async void ViewModel_RegistrationNotSuccessful(object sender, EventArgs e)
        {
            await DisplayAlert("Registration failed!", "Please Try again.", "OK");
        }

        private async void ViewModel_RegistrationSuccessful(object sender, EventArgs e)
        {
            await DisplayAlert("Registration successful!", string.Empty, "OK");
            await Navigation.PushModalAsync(new NavigationPage(new ViewLogin()));
        }

        private async void SendToLogin(object sender, EventArgs e)
        {
            if (Navigation.ModalStack.Any()) await Navigation.PopModalAsync();

            await Navigation.PushModalAsync(new NavigationPage(new ViewLogin()));
        }
    }
}