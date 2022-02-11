using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace changeMac
{
    class Const
    {
        public struct App
        {
            public const string VERSION = "0.01";
            public const string REG_MAC =  @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002BE10318}";
            public const string SQL_GETADAPTER = "SELECT * FROM Win32_NetworkAdapter WHERE Name = '{0}'";
        }

    }
}
