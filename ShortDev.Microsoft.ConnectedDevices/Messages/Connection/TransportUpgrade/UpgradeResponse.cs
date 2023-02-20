﻿using ShortDev.Networking;
using System;

namespace ShortDev.Microsoft.ConnectedDevices.Messages.Connection.TransportUpgrade;

/// <summary>
/// This message transports the upgrade response.
/// </summary>
public sealed class UpgradeResponse : ICdpPayload<UpgradeResponse>
{
    /// <summary>
    /// A length-prefixed list of endpoint structures (see following) that are provided by each transport on the host device.
    /// </summary>
    public required HostEndpoint[] HostEndpoints { get; init; }
    public required EndpointMetadata[] Endpoints { get; init; }

    public static UpgradeResponse Parse(EndianReader reader)
    {
        throw new NotImplementedException();
    }

    public void Write(EndianWriter writer)
    {
        writer.Write((ushort)HostEndpoints.Length);
        foreach (var endpoint in HostEndpoints)
        {
            writer.WriteWithLength(endpoint.Host);
            writer.WriteWithLength(endpoint.Service);
            writer.Write((ushort)endpoint.Type);
        }

        EndpointMetadata.WriteArray(writer, Endpoints);
    }
}
