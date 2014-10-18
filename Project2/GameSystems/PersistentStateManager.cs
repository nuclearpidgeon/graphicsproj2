﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.GameSystems
{
    public enum Quality { Low, Medium, High };

    class PersistentStateManager
    {
        public static Quality textureQuality = Quality.High;
        public static Quality lightingQuality = Quality.High;
        public static double accelSensitivity = 1.0f;

    }
}