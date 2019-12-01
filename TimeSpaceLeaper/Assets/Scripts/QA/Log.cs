using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LAB
{
    public static class Log
    {
        public static void Message(string msg)
        {
            Debug.Log(msg);
        }

        public static void Error(string msg)
        {
            Debug.LogError(msg);
        }

        public static void Warning(string msg)
        {
            Debug.LogWarning(msg);
        }

        public static void TypeCastError(Type type)
        {
            Error(String.Format(@"Cannot convert to type '{0}'", type.ToString()));
        }
    }
}
