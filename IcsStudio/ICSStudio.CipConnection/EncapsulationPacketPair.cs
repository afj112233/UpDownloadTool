using System;
using System.Diagnostics;
using System.Threading;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.CipConnection
{
    public enum MatchResultTypes
    {
        Waiting,
        Matched,
        TimeOut,
        Dropped
    }

    public class EncapsulationPacketPair
    {
        private readonly int _millisecondsTimeout;

        public EncapsulationPacketPair(EncapsulationPacket request, int millisecondsTimeout = 30000)
        {
            Request = request;
            MatchResult = MatchResultTypes.Waiting;
            MatchedSemaphoreSlim = new SemaphoreSlim(0);

            _millisecondsTimeout = millisecondsTimeout;
        }

        public EncapsulationPacket Request { get; }
        public EncapsulationPacket Response { get; private set; }

        public MatchResultTypes MatchResult { get; private set; }

        public SemaphoreSlim MatchedSemaphoreSlim { get; }

        public bool TryMatch(EncapsulationPacket response)
        {
            if (MatchResult == MatchResultTypes.Waiting)
            {
                var result = Request.TryMatch(response);
                if (result)
                {
                    Response = response;
                    MatchResult = MatchResultTypes.Matched;
                    MatchedSemaphoreSlim.Release();
                }

                return result;
            }

            return false;
        }

        public void CheckTimeOut()
        {
            if (MatchResult == MatchResultTypes.Waiting && Request.PacketTime != DateTime.MinValue)
            {
                var now = DateTime.Now;
                var timeSpan = now - Request.PacketTime;
                if (timeSpan.TotalMilliseconds > _millisecondsTimeout)
                {
                    MatchResult = MatchResultTypes.TimeOut;
                    MatchedSemaphoreSlim.Release();

                    Debug.WriteLine(Request.PacketHeader.GetSenderContext()
                                    + ": " + Request.PacketTime.ToString("HH:mm:ss.ffff")
                                    + " " + now.ToString("HH:mm:ss.ffff"));
                }
            }
        }

        public void Drop()
        {
            if (MatchResult == MatchResultTypes.Waiting)
            {
                MatchResult = MatchResultTypes.Dropped;
                MatchedSemaphoreSlim.Release();
            }
        }
    }
}