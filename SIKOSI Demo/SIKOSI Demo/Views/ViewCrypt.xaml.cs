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
using SIKOSI.Sample02.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SIKOSI.Sample02.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewCrypt : SikosiContentPage<ViewModelCrypt>
    {
        public ViewCrypt()
        {
            InitializeComponent();

            ViewModel.EncryptionSuccessful -= ViewModelOnEncryptionSuccessful;
            ViewModel.EncryptionNotSuccessful -= ViewModelOnEncryptionNotSuccessful;
            ViewModel.SendingEncryptedFileSuccessful -= ViewModelOnSendingEncryptedFileSuccessful;
            ViewModel.SendingEncryptedFileNotSuccessful -= ViewModelOnSendingEncryptedFileNotSuccessful;
            ViewModel.EncryptionSuccessful += ViewModelOnEncryptionSuccessful;
            ViewModel.EncryptionNotSuccessful += ViewModelOnEncryptionNotSuccessful;
            ViewModel.SendingEncryptedFileSuccessful += ViewModelOnSendingEncryptedFileSuccessful;
            ViewModel.SendingEncryptedFileNotSuccessful += ViewModelOnSendingEncryptedFileNotSuccessful;

            ViewModel.FilePicked += ViewModel_FilePicked;

            FilePickerButton.IsEnabled = false;
            BtnEncrypt.IsEnabled = false;
            BtnSendEncryptedFileToServer.IsEnabled = false;

            ViewModel.OnInitialized();
        }

        private async void ViewModelOnSendingEncryptedFileNotSuccessful(object sender, EventArgs e)
        {
            await DisplayAlert("Sending encrypted file failed", "Something went wrong while sending the encrypted file to the server.", "OK");
        }

        private async void ViewModelOnSendingEncryptedFileSuccessful(object sender, EventArgs e)
        {
            await DisplayAlert("Sending successful", "Sending the encrypted file to the server was successful", "OK");
        }

        private async void ViewModelOnEncryptionNotSuccessful(object sender, EventArgs e)
        {
            await DisplayAlert("Encryption failed", "Something went wrong while encrypting the file!", "OK");
        }

        private async void ViewModelOnEncryptionSuccessful(object sender, EventArgs e)
        {
            await DisplayAlert("Encryption successful", "The file was successfully encrypted!", "OK");

            BtnSendEncryptedFileToServer.IsEnabled = true;
        }


        private void ViewModel_FilePicked(object sender, EventArgs e)
        {
            LblSelectedFile.IsVisible = true;
            LblFileName.IsVisible = true;

            BtnEncrypt.IsEnabled = true;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilePickerButton.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue);
        }
    }
}