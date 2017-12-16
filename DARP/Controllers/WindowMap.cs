using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DARP.Controllers
{
    public class WindowMap
    {
        public WindowMap(Type windowType, Type modelType)
        {
            WindowType = windowType;
            ModelType = modelType;
        }

        public Type WindowType { get; }
        public Type ModelType { get; }
    }

    public class WindowMap<TWindow, TModel> : WindowMap
        where TWindow : Window, new()
    {
        public WindowMap() : base(typeof(TWindow), typeof(TModel))
        {
        }
    }
}
