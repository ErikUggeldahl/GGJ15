using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

namespace MPInput
{
    [Serializable]
    public class MP_InputConfig : ScriptableObject
    {
        public List<MP_InputAction> InputActions = new List<MP_InputAction>();
    }
}
