﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pici.Core.ConnectionManager
{
    public class GameSocketManagerStatics
    {
        public static readonly int BUFFER_SIZE = 1024;
        public static readonly int MAX_PACKET_SIZE = BUFFER_SIZE-4;
    }
}
