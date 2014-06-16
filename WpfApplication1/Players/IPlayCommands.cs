using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayServer.Player
{
    interface IPlayCommands
    {
         string Play();
         bool Stop();
         string Rewind();
         string Forward();
    }
}
