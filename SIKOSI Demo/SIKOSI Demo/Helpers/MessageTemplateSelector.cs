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
using SIKOSI.Exchange.Model;
using SIKOSI.Sample02.ViewModels;
using SIKOSI.Sample02.Views.Controls;
using Xamarin.Forms;

namespace SIKOSI.Sample02.Helpers
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        readonly DataTemplate _incomingTemplate;
        readonly DataTemplate _outgoingTemplate;

        public MessageTemplateSelector()
        {
            _incomingTemplate = new DataTemplate(typeof(IncomingChatMessage));
            _outgoingTemplate = new DataTemplate(typeof(OutgoingChatMessage));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item == null || !(item is ExChatMessage))
                return null;

            var message = (ExChatMessage) item;
            return (message.Author.Id == ViewModelBase.LocalUser.Id) ? _outgoingTemplate : _incomingTemplate;
        }
    }
}