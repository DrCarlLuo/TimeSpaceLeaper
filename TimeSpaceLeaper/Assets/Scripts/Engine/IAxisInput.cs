using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAB.Engine
{
    interface IAxisInput
    {
        bool Enabled { get; set; }
        float Value { get; }
    }
}
