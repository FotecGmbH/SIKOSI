﻿// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.11.2020 07:43
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SIKOSI.Sample02.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingChatMessage : ViewCell
    {
        public IncomingChatMessage()
        {
            InitializeComponent();
        }
    }
}