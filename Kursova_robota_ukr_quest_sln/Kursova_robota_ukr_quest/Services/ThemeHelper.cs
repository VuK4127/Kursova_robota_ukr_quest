using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Kursova_robota_ukr_quest
{
    public static class ThemeHelper
    {
        public static void ApplyTheme(string themeFileName)
        {
            var app = Application.Current;
            var existing = app.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Theme"));

            if (existing != null)
                app.Resources.MergedDictionaries.Remove(existing);

            app.Resources.MergedDictionaries.Insert(0, new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Resources/{themeFileName}", UriKind.Absolute)
            });
        }

        public static ImageSource GetDefaultAvatar()
        {
            // Generate a simple gray circle with person silhouette as DrawingImage
            var drawing = new DrawingGroup();

            // Background circle
            var bgGeo = new EllipseGeometry(new System.Windows.Point(50, 50), 50, 50);
            drawing.Children.Add(new GeometryDrawing(
                new SolidColorBrush(Color.FromRgb(158, 158, 158)), null, bgGeo));

            // Head
            var headGeo = new EllipseGeometry(new System.Windows.Point(50, 38), 16, 16);
            drawing.Children.Add(new GeometryDrawing(
                new SolidColorBrush(Color.FromRgb(220, 220, 220)), null, headGeo));

            // Body
            var bodyGeo = new EllipseGeometry(new System.Windows.Point(50, 76), 26, 20);
            drawing.Children.Add(new GeometryDrawing(
                new SolidColorBrush(Color.FromRgb(220, 220, 220)), null, bodyGeo));

            return new DrawingImage(drawing);
        }
    }
}
