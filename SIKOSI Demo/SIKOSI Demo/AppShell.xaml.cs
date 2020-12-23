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
using SIKOSI.Sample02.Views;
using Xamarin.Forms;

namespace SIKOSI.Sample02
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("viewCrypt", typeof(ViewCrypt));
            Routing.RegisterRoute("viewLogin", typeof(ViewLogin));
            Routing.RegisterRoute("viewRegistration", typeof(ViewRegistration));
            Routing.RegisterRoute("viewChatOverview", typeof(ViewChatOverview));
        }
    }
}