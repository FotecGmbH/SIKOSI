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

namespace SIKOSI.Sample02.Views.Controls
{
#pragma warning disable CS0108
    public class ExtendedEditor : Editor
    {
        public static BindableProperty PlaceholderProperty
            = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ExtendedEditor));

        public static BindableProperty PlaceholderColorProperty
            = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(ExtendedEditor), Color.LightGray);

        public static BindableProperty HasRoundedCornerProperty
            = BindableProperty.Create(nameof(HasRoundedCorner), typeof(bool), typeof(ExtendedEditor), false);

        public static BindableProperty IsExpandableProperty
            = BindableProperty.Create(nameof(IsExpandable), typeof(bool), typeof(ExtendedEditor), false);

        public static BindableProperty BorderColorProperty
            = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ExtendedEditor), Color.LightGray);

        public ExtendedEditor()
        {
            TextChanged += OnTextChanged;
        }

        #region Properties

        public bool IsExpandable
        {
            get { return (bool) GetValue(IsExpandableProperty); }
            set { SetValue(IsExpandableProperty, value); }
        }

        public bool HasRoundedCorner
        {
            get { return (bool) GetValue(HasRoundedCornerProperty); }
            set { SetValue(HasRoundedCornerProperty, value); }
        }

        public string Placeholder
        {
            get { return (string) GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public Color PlaceholderColor
        {
            get { return (Color) GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color) GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        #endregion


        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsExpandable) InvalidateMeasure();
        }

        ~ExtendedEditor()
        {
            TextChanged -= OnTextChanged;
        }
    }
#pragma warning restore CS0108
}