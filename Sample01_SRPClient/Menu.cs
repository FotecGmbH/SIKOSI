// (C) 2019 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created:        13.09.2019 10:15
// Developer:      Gregor Faiman
// Project         SIKOSI
//
// Released under GPL-3.0-only
namespace Sample01_SRPClient
{
    using System.Collections.Generic;

    public class Menu
    {
        public Menu(List<MenuEntry> entries)
        {
            this.Entries = entries;
        }

        public List<MenuEntry> Entries
        {
            get;
            set;
        }
    }
}
