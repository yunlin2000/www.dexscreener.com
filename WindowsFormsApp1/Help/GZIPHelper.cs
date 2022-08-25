using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcashface.EDCode
{
   public class GZIPHelper
    {
        public static Byte[] D(byte[] bytes)
        {

            if (bytes.Count() > 0)
                try
                {
                    using (var compressStream = new MemoryStream(bytes))
                    {
                        using (var zipStream = new GZipStream(compressStream, System.IO.Compression.CompressionMode.Decompress))
                        {
                            using (var resultStream = new MemoryStream())
                            {
                                zipStream.CopyTo(resultStream);
                                return resultStream.ToArray();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            return null;
        }
    }
}
