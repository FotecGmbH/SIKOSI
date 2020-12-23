// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.11.2020 07:43
// Developer      Georg Wernitznig
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System.ComponentModel;
using Android.Content;
using Android.Graphics.Drawables;
using SIKOSI.Sample02.Views.Controls;
using SIKOSI_Demo.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedEditor), typeof(ExtendedEditorRenderer))]

namespace SIKOSI_Demo.Droid.CustomRenderer
{
    public class ExtendedEditorRenderer : EditorRenderer
    {
        ExtendedEditor _customControl;
        bool initial = true;
        Drawable originalBackground;

        public ExtendedEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                if (initial)
                {
                    originalBackground = Control.Background;
                    initial = false;
                }

                Control.SetMaxLines(5);
            }

            if (e.NewElement != null)
            {
                _customControl = (ExtendedEditor) Element;
                if (_customControl.HasRoundedCorner) ApplyBorder();

                if (!string.IsNullOrEmpty(_customControl.Placeholder))
                {
                    Control.Hint = _customControl.Placeholder;
                    Control.SetHintTextColor(_customControl.PlaceholderColor.ToAndroid());
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            _customControl = (ExtendedEditor) Element;

            if (ExtendedEditor.PlaceholderProperty.PropertyName == e.PropertyName)
            {
                Control.Hint = _customControl.Placeholder;
            }
            else if (ExtendedEditor.PlaceholderColorProperty.PropertyName == e.PropertyName)
            {
                Control.SetHintTextColor(_customControl.PlaceholderColor.ToAndroid());
            }
            else if (ExtendedEditor.HasRoundedCornerProperty.PropertyName == e.PropertyName)
            {
                if (_customControl.HasRoundedCorner)
                    ApplyBorder();
                else
                    Control.Background = originalBackground;
            }
            else if (ExtendedEditor.BorderColorProperty.PropertyName == e.PropertyName)
            {
                ApplyBorder();
            }
        }

        void ApplyBorder()
        {
            GradientDrawable gd = new GradientDrawable();
            gd.SetCornerRadius(10);
            if (_customControl != null)
                gd.SetStroke(2, _customControl.BorderColor.ToAndroid());
            else
                gd.SetStroke(2, Color.Black.ToAndroid());
            Control.Background = gd;
        }
    }
}