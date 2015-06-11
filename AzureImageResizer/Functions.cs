using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using ImageProcessor;
using System.Drawing;
using ImageProcessor.Imaging.Formats;

namespace AzureImageResizer
{
    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }

        public static void Create960x([BlobTriggerAttribute("barakuba-input/{name}")] Stream input, [BlobAttribute("barakuba/960x/{name}", FileAccess.Write)] Stream output)
        {
            Resize(960, input, output);
        }

        public static void Create640x([BlobTriggerAttribute("barakuba-input/{name}")] Stream input, [BlobAttribute("barakuba/640x/{name}", FileAccess.Write)] Stream output)
        {
            Resize(640, input, output);
        }

        public static void Create240x([BlobTriggerAttribute("barakuba-input/{name}")] Stream input, [BlobAttribute("barakuba/240x/{name}", FileAccess.Write)] Stream output)
        {
            Resize(240, input, output);
        }

        private static void Resize(int width, Stream input, Stream output)
        {
            using (var factory = new ImageFactory(preserveExifData: true))
            using (var memory = new MemoryStream())
            {
                factory.Load(input)
                    .Resize(new Size(width, 0))
                    .Format(new JpegFormat { Quality = 75 })
                    .Save(memory);

                memory.CopyTo(output);
            }
        }
    }
}
