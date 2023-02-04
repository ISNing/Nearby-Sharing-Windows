﻿using ShortDev.Microsoft.ConnectedDevices.Messages.Session;
using ShortDev.Microsoft.ConnectedDevices.Platforms;
using ShortDev.Networking;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShortDev.Microsoft.ConnectedDevices.Messages.Control;

/// <summary>
/// Provides the interface between a <see cref="IChannelMessageHandler"/> and a <see cref="CdpSession"/>. <br/>
/// Every handler / app has a unique <see cref="CdpSocket"/> managed from within this channel.
/// </summary>
public sealed class CdpChannel : IDisposable
{
    internal CdpChannel(CdpSession session, ulong channelId, IChannelMessageHandler handler, CdpSocket socket)
    {
        Session = session;
        ChannelId = channelId;
        MessageHandler = handler;
        Socket = socket;
    }

    /// <summary>
    /// Get's the corresponding <see cref="CdpSession"/>. <br/>
    /// <br/>
    /// <inheritdoc cref="CdpSession"/>
    /// </summary>
    public CdpSession Session { get; }

    /// <summary>
    /// Get's the corresponding <see cref="CdpSocket"/>. <br/>
    /// <br/>
    /// <inheritdoc cref="CdpSocket" />
    /// </summary>
    public CdpSocket Socket { get; }

    /// <summary>
    /// Get's the corresponding <see cref="IChannelMessageHandler"/>. <br/>
    /// (See <see cref="CdpAppBase"/>)
    /// </summary>
    public IChannelMessageHandler MessageHandler { get; }

    /// <summary>
    /// Get's the unique id for the channel. <br/>
    /// The id is unique as long as the channel is active.
    /// </summary>
    public ulong ChannelId { get; }

    public async ValueTask HandleMessageAsync(CdpMessage msg)
        => await MessageHandler.HandleMessageAsync(msg);

    public void SendMessage(CommonHeader oldHeader, BodyCallback bodyCallback)
    {
        lock (this)
        {
            // ToDo: Better way!!
            SendMessage(
                 ++oldHeader.SequenceNumber,
                 bodyCallback
            );
        }
    }

    public void SendMessage(uint sequenceNumber, BodyCallback bodyCallback)
    {
        // ToDo
        //if (Session.Cryptor == null)
        //    throw new InvalidOperationException("Invalid session state!");

        lock (this)
        {
            CommonHeader header = new();
            header.Type = MessageType.Session;

            header.SessionId = Session.GetSessionId();
            header.ChannelId = ChannelId;

            header.SequenceNumber = sequenceNumber;
            // ToDo: "AdditionalHeaders" ... "RequestID" ??

            Session.SendMessage(Socket, header, writer =>
            {
                new SessionFragmentHeader()
                {
                    MessageId = 0
                }.Write(writer);
                bodyCallback(writer);
            });
        }
    }

    void IDisposable.Dispose()
        => Dispose();

    public void Dispose(bool closeSession = false, bool closeSocket = false)
    {
        Session.UnregisterChannel(this);
        if (closeSocket)
            Socket.Dispose();
        if (closeSession)
            Session.Dispose();
    }
}
