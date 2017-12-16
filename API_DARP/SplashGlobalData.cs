using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Splash
{
    public static class SplashGlobalData
    {

        private static Dictionary<string, object> _globalData = new Dictionary<string, object>();

        public static void SetSplashData(string data, object value)
        {
            if (!_globalData.ContainsKey(data))
                _globalData.Add(data, value);
            else
            {
                _globalData[data] = value;
            }
        }


        public static bool IsStoredSplashData(string data)
        {
            if (_globalData.ContainsKey(data))
                return true;
            return false;
        }


        public static object GetSplashData<T>(string data)
        {
            if (IsStoredSplashData(data))
                return _globalData[data];

            return default(T);
        }

    }
}
