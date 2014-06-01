using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Player
{
    interface IPlayCommands
    {
         void Play();
         void Stop();
         void Rewind();
         void Forward();
    }
}
