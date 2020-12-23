using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using SIKOSI_Demo.iOS.CustomRenderer;
using SIKOSI_Demo.Views.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedEditor), typeof(ExtendedEditorRenderer))]
namespace SIKOSI_Demo.iOS.CustomRenderer
{
    public class ExtendedEditorRenderer : EditorRenderer
    {
        private int _prevLines = 0;
        private double _previousHeight = -1;
        UILabel _placeholderLabel;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                if (_placeholderLabel == null)
                {
                    CreatePlaceholder();
                }


            }

            if (e.NewElement != null)
            {
                var customControl = (ExtendedEditor)e.NewElement;

                if (customControl.IsExpandable)
                    Control.ScrollEnabled = false;
                else
                    Control.ScrollEnabled = true;

                if (customControl.HasRoundedCorner)
                    Control.Layer.CornerRadius = 5;
                else
                    Control.Layer.CornerRadius = 0;
            }


        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var customControl = (ExtendedEditor)Element;


            if (e.PropertyName == Editor.TextProperty.PropertyName)
            {
                if (customControl.IsExpandable)
                {
                    CGSize size = Control.Text.StringSize(Control.Font, Control.Frame.Size, UILineBreakMode.WordWrap);

                    int numLines = (int)(size.Height / Control.Font.LineHeight);

                    if (_prevLines > numLines)
                    {
                        customControl.HeightRequest = -1;

                    }
                    else if (string.IsNullOrEmpty(Control.Text))
                    {
                        customControl.HeightRequest = -1;
                    }

                    _prevLines = numLines;
                }

                _placeholderLabel.Hidden = !string.IsNullOrEmpty(Control.Text);
            }
            else if (ExtendedEditor.PlaceholderProperty.PropertyName == e.PropertyName)
            {
                _placeholderLabel.Text = customControl.Placeholder;
            }
            else if (ExtendedEditor.PlaceholderColorProperty.PropertyName == e.PropertyName)
            {
                _placeholderLabel.TextColor = customControl.PlaceholderColor.ToUIColor();
            }
            else if (ExtendedEditor.HasRoundedCornerProperty.PropertyName == e.PropertyName)
            {
                if (customControl.HasRoundedCorner)
                    Control.Layer.CornerRadius = 5;
                else
                    Control.Layer.CornerRadius = 0;                
            }
            else if (ExtendedEditor.IsExpandableProperty.PropertyName == e.PropertyName)
            {
                if (customControl.IsExpandable)
                    Control.ScrollEnabled = false;
                else
                    Control.ScrollEnabled = true;
            }
            else if (ExtendedEditor.HeightProperty.PropertyName == e.PropertyName)
            {
                if (customControl.IsExpandable)
                {
                    CGSize size = Control.Text.StringSize(Control.Font, Control.Frame.Size, UILineBreakMode.WordWrap);

                    int numLines = (int)(size.Height / Control.Font.LineHeight);
                    if (numLines >= 5)
                    {
                        Control.ScrollEnabled = true;

                        customControl.HeightRequest = _previousHeight;

                    }
                    else
                    {
                        Control.ScrollEnabled = false;
                        _previousHeight = customControl.Height;

                    }
                }
            }
            else if (ExtendedEditor.BorderColorProperty.PropertyName == e.PropertyName)
            {                
                Control.Layer.BorderColor = customControl.BorderColor.ToCGColor();
            }
        }

        public void CreatePlaceholder()
        {
            var element = Element as ExtendedEditor;

            _placeholderLabel = new UILabel
            {
                Text = element?.Placeholder,
                TextColor = element.PlaceholderColor.ToUIColor(),
                BackgroundColor = UIColor.Clear
            };

            var edgeInsets = Control.TextContainerInset;
            var lineFragmentPadding = Control.TextContainer.LineFragmentPadding;

            Control.AddSubview(_placeholderLabel);

            var vConstraints = NSLayoutConstraint.FromVisualFormat(
                "V:|-" + edgeInsets.Top + "-[PlaceholderLabel]-" + edgeInsets.Bottom + "-|", 0, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { _placeholderLabel }, new NSObject[] { new NSString("PlaceholderLabel") })
            );

            var hConstraints = NSLayoutConstraint.FromVisualFormat(
                "H:|-" + lineFragmentPadding + "-[PlaceholderLabel]-" + lineFragmentPadding + "-|",
                0, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { _placeholderLabel }, new NSObject[] { new NSString("PlaceholderLabel") })
            );

            _placeholderLabel.TranslatesAutoresizingMaskIntoConstraints = false;

            Control.AddConstraints(hConstraints);
            Control.AddConstraints(vConstraints);
        }
    }
}