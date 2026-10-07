// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests;


/// <summary> An in-memory handler: each request is answered by <c>respond(request, attemptIndex)</c>, which may also throw to simulate a network failure. </summary>
internal sealed class StubHandler( Func<HttpRequestMessage, int, HttpResponseMessage> respond ) : HttpMessageHandler
{
    private int __count;

    public readonly List<HttpRequestMessage> Requests = [];
    public readonly List<string?>            Bodies   = [];
    public          int                      Count => Volatile.Read(ref __count);


    protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken token )
    {
        token.ThrowIfCancellationRequested();
        int index = Interlocked.Increment(ref __count) - 1;

        lock ( Requests ) { Requests.Add(request); }

        string? body = request.Content is null
                           ? null
                           : await request.Content.ReadAsStringAsync(token).ConfigureAwait(false);

        lock ( Bodies ) { Bodies.Add(body); }

        HttpResponseMessage response = respond(request, index);
        response.RequestMessage = request;
        return response;
    }


    public static HttpResponseMessage Text( HttpStatusCode status, string body ) => new(status) { Content = new StringContent(body, Encoding.UTF8) };
}



/// <summary> A minimal HTTP/1.1 server on a loopback port (keep-alive, Content-Length bodies), answering each request with <c>respond(request, index)</c>. </summary>
internal sealed class LoopbackServer : IAsyncDisposable
{
    private readonly TcpListener                  __listener = new(IPAddress.Loopback, 0);
    private readonly CancellationTokenSource      __cts      = new();
    private readonly Func<Request, int, Response> __respond;
    private readonly Task                         __loop;

    public readonly List<Request> Requests = [];
    public          Uri           Url { get; }


    public LoopbackServer( Func<Request, int, Response> respond )
    {
        __respond = respond;
        __listener.Start();
        Url    = new Uri($"http://127.0.0.1:{( (IPEndPoint)__listener.LocalEndpoint ).Port}/");
        __loop = Task.Run(Accept);
    }


    public async ValueTask DisposeAsync()
    {
        await __cts.CancelAsync();
        __listener.Stop();

        try { await __loop; }
        catch ( Exception ) { }

        __cts.Dispose();
    }


    /// <summary> A port with nothing listening on it. </summary>
    public static Uri UnusedUrl()
    {
        TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        int port = ( (IPEndPoint)listener.LocalEndpoint ).Port;
        listener.Stop();
        return new Uri($"http://127.0.0.1:{port}/");
    }


    private async Task Accept()
    {
        while ( !__cts.IsCancellationRequested )
        {
            TcpClient client;

            try { client = await __listener.AcceptTcpClientAsync(__cts.Token); }
            catch ( Exception ) { return; }

            _ = Task.Run(() => Serve(client));
        }
    }


    private async Task Serve( TcpClient client )
    {
        using ( client )
        {
            NetworkStream stream  = client.GetStream();
            List<byte>    pending = [];
            byte[]        chunk   = new byte[8192];

            try
            {
                while ( true )
                {
                    // Read until the end of the headers.
                    int headerEnd;

                    while ( ( headerEnd = IndexOfHeaderEnd(pending) ) < 0 )
                    {
                        int read = await stream.ReadAsync(chunk, __cts.Token);
                        if ( read == 0 ) { return; }

                        pending.AddRange(chunk.AsSpan(0, read));
                    }

                    string[]                   lines   = Encoding.ASCII.GetString(pending.GetRange(0, headerEnd).ToArray()).Split("\r\n");
                    string[]                   start   = lines[0].Split(' ');
                    Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase);

                    foreach ( string line in lines[1..] )
                    {
                        int colon = line.IndexOf(':');
                        if ( colon > 0 ) { headers[line[..colon].Trim()] = line[( colon + 1 )..].Trim(); }
                    }

                    pending.RemoveRange(0, headerEnd + 4);

                    int length = headers.TryGetValue("Content-Length", out string? value)
                                     ? int.Parse(value)
                                     : 0;

                    while ( pending.Count < length )
                    {
                        int read = await stream.ReadAsync(chunk, __cts.Token);
                        if ( read == 0 ) { return; }

                        pending.AddRange(chunk.AsSpan(0, read));
                    }

                    byte[] body = [.. pending.GetRange(0, length)];
                    pending.RemoveRange(0, length);

                    Request request = new(start[0], start[1], headers, body);
                    int     index;

                    lock ( Requests )
                    {
                        index = Requests.Count;
                        Requests.Add(request);
                    }

                    Response      response = __respond(request, index);
                    StringBuilder head     = new($"HTTP/1.1 {response.Status} X\r\nContent-Length: {response.Body.Length}\r\n");

                    if ( response.Headers is not null )
                    {
                        foreach ( ( string key, string headerValue ) in response.Headers ) { head.Append($"{key}: {headerValue}\r\n"); }
                    }

                    head.Append("\r\n");

                    await stream.WriteAsync(Encoding.ASCII.GetBytes(head.ToString()), __cts.Token);
                    await stream.WriteAsync(response.Body,                            __cts.Token);
                    await stream.FlushAsync(__cts.Token);
                }
            }
            catch ( Exception ) { }
        }
    }


    private static int IndexOfHeaderEnd( List<byte> bytes )
    {
        for ( int i = 3; i < bytes.Count; i++ )
        {
            if ( bytes[i - 3] == '\r' && bytes[i - 2] == '\n' && bytes[i - 1] == '\r' && bytes[i] == '\n' ) { return i - 3; }
        }

        return -1;
    }



    public sealed record Request( string Method, string Path, Dictionary<string, string> Headers, byte[] Body )
    {
        public string Text => Encoding.UTF8.GetString(Body);
    }



    public sealed record Response( int Status, byte[] Body, Dictionary<string, string>? Headers = null )
    {
        public static Response Of( int status, string body, Dictionary<string, string>? headers = null ) => new(status, Encoding.UTF8.GetBytes(body), headers);
    }
}
