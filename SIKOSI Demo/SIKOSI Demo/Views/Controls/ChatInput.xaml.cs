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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SIKOSI.Sample02.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatInput : ContentView
    {
        public ChatInput()
        {
            InitializeComponent();


            if (Device.RuntimePlatform == Device.iOS) SetBinding(HeightRequestProperty, new Binding("Height", BindingMode.OneWay, null, null, null, TextEditor));
        }

        public void Handle_Completed(object sender, EventArgs e)
        {
            //(this.Parent.Parent.BindingContext as ViewModelChat).CmdSendMessage.Execute(null);
            //TextEntry.Focus();

            var p = Parent.Parent as ViewChatOverview;
            if (!(p is null)) p.CmdScrollToMessage.Execute(null);

            TextEditor.Focus();
        }

        public void UnFocusEntry()
        {
            //TextEntry.Unfocus();
            TextEditor.Unfocus();
        }
    }
}