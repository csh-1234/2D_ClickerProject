using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManagement
{
    private Dictionary<string, BuffInfo> _activeBuffs = new Dictionary<string, BuffInfo>();

    public class BuffInfo
    {
        public string BuffId { get; set; } 
        public Stats BuffStats { get; set; }
        public float EndTime { get; set; }
    }
}
