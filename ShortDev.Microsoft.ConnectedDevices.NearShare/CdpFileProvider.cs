﻿using System.Text;

namespace ShortDev.Microsoft.ConnectedDevices.NearShare;

public sealed class CdpFileProvider
{
    readonly ReadOnlyMemory<byte> _buffer;
    private CdpFileProvider(string fileName, ReadOnlyMemory<byte> buffer)
    {
        FileName = fileName;
        _buffer = buffer;
    }

    public static CdpFileProvider FromContent(string fileName, string content)
        => FromContent(fileName, content, Encoding.UTF8);

    public static CdpFileProvider FromContent(string fileName, string content, Encoding encoding)
    {
        var buffer = encoding.GetBytes(content);
        return FromBuffer(fileName, buffer);
    }

    public static CdpFileProvider FromBuffer(string fileName, ReadOnlyMemory<byte> buffer)
        => new(fileName, buffer);

    public string FileName { get; }

    public ulong FileSize
        => (ulong)_buffer.Length;

    public ReadOnlySpan<byte> ReadBlob(ulong start, uint length)
        => _buffer.Slice((int)start, (int)length).Span;
}
