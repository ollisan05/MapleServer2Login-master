using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using MaplePacketLib2.Crypto;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Enums;
using MapleServer2.Extensions;
using Microsoft.Extensions.Logging;

namespace MapleServer2.Network {
    public abstract class Session : IDisposable {
        public const uint VERSION = 12;

        private const int BUFFER_SIZE = 1024;
        private const uint BLOCK_IV = 12; // TODO: should this be variable

        protected abstract SessionType Type { get; }

        public EventHandler<string> OnError;
        public EventHandler<Packet> OnPacket;

        private uint siv;
        private uint riv;

        private Thread sessionThread;
        private TcpClient client;
        private NetworkStream networkStream;
        private MapleStream mapleStream;
        private MapleCipher sendCipher;
        private MapleCipher recvCipher;

        // These are unencrypted packets, scheduled to be sent on a single thread.
        private readonly Queue<byte[]> sendQueue;
        private readonly byte[] recvBuffer;
        private readonly CancellationTokenSource source;

        protected readonly ILogger logger;

        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        protected Session(ILogger<Session> logger) {
            this.logger = logger;
            this.sendQueue = new Queue<byte[]>();
            this.recvBuffer = new byte[BUFFER_SIZE];
            this.source = new CancellationTokenSource();
        }

        public void Init([NotNull] TcpClient client) {
            // Allow client to close immediately
            client.LingerState = new LingerOption(true, 0);

            byte[] sivBytes = new byte[4];
            byte[] rivBytes = new byte[4];
            rng.GetBytes(sivBytes);
            rng.GetBytes(rivBytes);
            this.siv = BitConverter.ToUInt32(sivBytes);
            this.riv = BitConverter.ToUInt32(rivBytes);

            this.client = client;
            this.networkStream = client.GetStream();
            this.mapleStream = new MapleStream();
            this.sendCipher = MapleCipher.Encryptor(VERSION, siv, BLOCK_IV);
            this.recvCipher = MapleCipher.Decryptor(VERSION, riv, BLOCK_IV);
        }

        public void Dispose() {
            source.Cancel();
            client?.Dispose();
        }

        public void Disconnect() {
            source.Cancel();
            if (Connected()) {
                // Must close socket before network stream to prevent lingering
                client.Client.Close();
                client.Close();
                logger.Debug($"Disconnected client.");
            }
        }

        public bool Connected() {
            TcpConnectionInformation info = IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .SingleOrDefault(c => c.LocalEndPoint.Equals(client.Client.LocalEndPoint));

            return info?.State == TcpState.Established;
        }

        public void Start() {
            if (sessionThread != null) {
                throw new ArgumentException("Session has already been started.");
            }

            sessionThread = new Thread(() => {
                try {
                    PerformHandshake();
                    StartRead();
                } catch (SystemException ex) {
                    logger.Trace($"Fatal error for session:{this}", ex);
                }

                Disconnect();
            });
            sessionThread.Start();
        }

        private void PerformHandshake() {
            if (client == null) {
                throw new InvalidOperationException("Cannot start a session without a client.");
            }

            var pWriter = PacketWriter.Of(SendOp.REQUEST_VERSION);
            pWriter.WriteUInt(VERSION);
            pWriter.WriteUInt(riv);
            pWriter.WriteUInt(siv);
            pWriter.WriteUInt(BLOCK_IV);
            pWriter.WriteByte((byte)Type);

            // No encryption for handshake
            Packet packet = sendCipher.WriteHeader(pWriter.ToArray());
            logger.Debug($"Handshake: {packet}");
            SendRaw(packet);
        }

        public void Send(params byte[] packet) {
            lock (sendQueue) {
                sendQueue.Enqueue(packet);
            }
        }

        public void Send(Packet packet) {
            lock (sendQueue) {
                sendQueue.Enqueue(packet.ToArray());
            }
        }

        private void StartRead() {
            while (!source.IsCancellationRequested) {
                // Perform Reads
                try {
                    int length = networkStream.Read(recvBuffer, 0, recvBuffer.Length);
                    if (length <= 0) {
                        continue;
                    }

                    mapleStream.Write(recvBuffer, 0, length);
                } catch (IOException ex) {
                    logger.Error("Exception reading from socket: ", ex);
                    return;
                }

                byte[] packetBuffer = mapleStream.Read();
                if (packetBuffer != null) {
                    Packet packet = recvCipher.Transform(packetBuffer);
                    if (packet.Reader().ReadShort() != 0x912) {
                        logger.Debug($"RECV ({packet.Length}): {packet}");
                    }
                    OnPacket?.Invoke(this, packet); // handle packet
                }

                // Perform Writes
                lock (sendQueue) {
                    while (sendQueue.TryDequeue(out byte[] packet)) {
                        SendInternal(packet);
                    }
                }
            }
        }

        private void SendInternal(byte[] packet) {
            logger.Debug($"SEND ({packet.Length}): {packet.ToHexString(' ')}");
            Packet encryptedPacket = sendCipher.Transform(packet);
            SendRaw(encryptedPacket);
        }

        private void SendRaw(Packet packet) {
            try {
                networkStream.Write(packet.Buffer, 0, packet.Length);
            } catch (IOException ex) {
                logger.Error("Exception writing to socket: ", ex);
                Disconnect();
            }
        }

        public override string ToString() {
            return $"{GetType().Name} from {client?.Client.RemoteEndPoint}";
        }
    }
}