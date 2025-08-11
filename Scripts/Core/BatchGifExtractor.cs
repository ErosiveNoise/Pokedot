using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using SixLabors.ImageSharp.Advanced;


class Program
{
    static void Main(string[] args)
    {
        string inputGif = "input.gif";
        string outputFolder = "frames";

        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        using Image<Rgba32> gif = Image.Load<Rgba32>(inputGif);

        for (int i = 0; i < gif.Frames.Count; i++)
        {
            var frame = gif.Frames[i];

            // Crea nuova immagine della dimensione del frame
            using var img = new Image<Rgba32>(frame.Width, frame.Height);

            // Copia pixel manualmente: 
            for (int y = 0; y < frame.Height; y++)
            {
                var sourceSpan = frame.PixelBuffer.DangerousGetRowSpan(y);
                var targetSpan = img.Frames.RootFrame.PixelBuffer.DangerousGetRowSpan(y);
                sourceSpan.CopyTo(targetSpan);
            }

            string outPath = Path.Combine(outputFolder, $"frame_{i}.png");
            img.Save(outPath);
        }
    }
}
