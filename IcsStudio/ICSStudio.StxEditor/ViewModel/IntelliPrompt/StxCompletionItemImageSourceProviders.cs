using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    public sealed class StxCompletionItemImageSourceProviders
    {
        static StxCompletionItemImageSourceProviders()
        {
        }

        private StxCompletionItemImageSourceProviders()
        {
        }

        public static ImageSource KeywordItemImage { get; } = CreateImageFromResource("keyword.png");
        public static ImageSource CodeSnippetItemImage { get; } = CreateImageFromResource("codesnippet.png");
        public static ImageSource TagItemImage { get; } = CreateImageFromResource("tag.png");
        public static ImageSource ProgramImage { get; } = CreateImageFromResource("program.png");

        private static ImageSource CreateImageFromResource(string imageName)
        {
            const string imagePath = "pack://application:,,,/ICSStudio.StxEditor;component/images/";

            if (imageName == null)
                throw new ArgumentNullException(nameof(imageName));
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath + imageName, UriKind.Absolute);
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
