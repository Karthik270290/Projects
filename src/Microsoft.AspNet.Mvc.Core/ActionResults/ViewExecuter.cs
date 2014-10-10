using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;

namespace Microsoft.AspNet.Mvc.Core
{
    public class ViewExecuter
    {
        private const int BufferSize = 1024;

        public static async Task ExecuteView([NotNull]IView view, 
            [NotNull] ActionContext context, 
            [NotNull] ViewDataDictionary viewData)
        {
            using (view as IDisposable)
            {
                var response = context.HttpContext.Response;

                response.ContentType = "text/html; charset=utf-8";
                var wrappedStream = new StreamWrapper(response.Body);
                var encoding = Encodings.UTF8EncodingWithoutBOM;
                using (var writer = new StreamWriter(wrappedStream, encoding, BufferSize, leaveOpen: true))
                {
                    try
                    {
                        var viewContext = new ViewContext(context, view, viewData, writer);
                        await view.RenderAsync(viewContext);
                    }
                    catch
                    {
                        // Need to prevent writes/flushes on dispose because the StreamWriter will flush even if
                        // nothing got written. This leads to a response going out on the wire prematurely in case an
                        // exception is being thrown inside the try catch block.
                        wrappedStream.BlockWrites = true;
                        throw;
                    }
                }
            }
        }

        private class StreamWrapper : Stream
        {
            private readonly Stream _wrappedStream;

            public StreamWrapper([NotNull] Stream stream)
            {
                _wrappedStream = stream;
            }

            public bool BlockWrites { get; set; }

            public override bool CanRead
            {
                get { return _wrappedStream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _wrappedStream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _wrappedStream.CanWrite; }
            }

            public override void Flush()
            {
                if (!BlockWrites)
                {
                    _wrappedStream.Flush();
                }
            }

            public override long Length
            {
                get { return _wrappedStream.Length; }
            }

            public override long Position
            {
                get
                {
                    return _wrappedStream.Position;
                }
                set
                {
                    _wrappedStream.Position = value;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _wrappedStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (!BlockWrites)
                {
                    _wrappedStream.Write(buffer, offset, count);
                }
            }
        }
    }
}
