using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayServer.Network
{
    public interface IMainPlayerSubject
    {
        void RegisterListener(IMainPlayerListener mainPlayerListener);
        void UnRegisterListener(IMainPlayerListener mainPlayerListener);
        void SendMessage(byte[] message);
    }
}
