using System;
using UnityEngine;

namespace FateGO.Alter.Managed
{
    public class MyWaitForSeconds
    {
        public MyWaitForSeconds(float seconds)
        {
            this.m_Seconds = seconds + Time.time;
        }
        internal float m_Seconds;
    }
}
