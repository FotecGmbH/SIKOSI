// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 16.12.2020 14:52
// Developer       Roman Jahn
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
    public partial class ViewChatOverview : SikosiContentPage<ViewModelChatOverview>
    {
        public ViewChatOverview()
        {
            InitializeComponent();
            CmdScrollToMessage = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (ViewModel.ViewedMessages?.Any() ?? false) ChatOverviewList.ScrollTo(ViewModel.ViewedMessages.Last(), ScrollToPosition.End, false);
                });
            });

            ViewModel.OnUsersReceived += ViewModel_OnUsersReceived;
        }

        #region Properties

        public Command CmdScrollToMessage { get; set; }

        #endregion

        private async void ViewModel_OnUsersReceived(object sender, EventArgs e)
        {
            //var x = ViewModel.AvailableUsers.LastOrDefault();
            //ChatOverviewList.ScrollTo(x, ScrollToPosition.Start, false);
            //InvalidateMeasure();
            //ForceLayout();
        }
    }
}