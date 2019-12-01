using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAB.Engine
{
    interface IButtonInput
    {
        bool Enabled { get; set; }
        bool KeyHold { get;}
        bool KeyDown { get;}
        bool KeyUp { get;}

    }
}
