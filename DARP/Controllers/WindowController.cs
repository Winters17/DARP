using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DARP.Controllers
{
    public class WindowController : IWindowController
    {
        public WindowController()
        {
            // Registro de vistas "por convención"
            RegisterViewsFromAssembly(Application.Current.GetType().Assembly);
        }

        public WindowController(IEnumerable<WindowMap> mappings)
        {
            // Registro de vistas "por configuración"
            RegisterViewsFromMappings(mappings);
        }

        public bool ShowWindow(object viewModel)
        {
            Type windowType;
            if (!mappings.TryGetValue(viewModel.GetType(), out windowType))
                throw new ArgumentException("No existe ventana para el tipo: " + viewModel.GetType().FullName, nameof(viewModel));

            Window window = (Window)Activator.CreateInstance(windowType);
            window.DataContext = viewModel;
            bool? result = window.ShowDialog();
            window.DataContext = null;

            return result == true;
        }

        public bool CloseWindow(Window w)
        {
            w.Close();
            return true;
        }

        private void RegisterViewsFromMappings(IEnumerable<WindowMap> mappings)
        {
            foreach (var mapping in mappings)
                this.mappings.Add(mapping.ModelType, mapping.WindowType);
        }

        private void RegisterViewsFromAssembly(Assembly appAssembly)
        {
            Func<Type, bool> isViewType = type => true
                && type.IsClass
                && type.IsAbstract == false
                && type.GetConstructor(Type.EmptyTypes)?.IsPublic == true
                && typeof(Window).IsAssignableFrom(type);

            Func<Type, Type> selectViewModelType = viewType =>
                Type.GetType(viewType.Namespace.Replace(".Views", ".ViewModels") + "." + viewType.Name.Replace("Window", "ViewModel"), false);

            IEnumerable<Type> viewTypes = appAssembly.GetTypes().Where(isViewType);

            IEnumerable<WindowMap> mappings =
                from viewType in viewTypes
                let viewModelType = selectViewModelType(viewType)
                where viewModelType != null
                select new WindowMap(viewType, viewModelType);

            foreach (var mapping in mappings)
                this.mappings.Add(mapping.ModelType, mapping.WindowType);
        }

        private void AddMapping(WindowMap mapping)
        {
            Debug.WriteLine($"Registering window: {mapping.ModelType} -> {mapping.WindowType}", "WindowController");
            mappings.Add(mapping.ModelType, mapping.WindowType);
        }

        private readonly IDictionary<Type, Type> mappings = new Dictionary<Type, Type>();

        public static WindowController Instance
        {
            get
            {
                return instance;
            }
            set
            {
                if (instance != null)
                    throw new InvalidOperationException("Ya hay una instancia de la clase Singleton");

                instance = value;
            }
        }

        private static WindowController instance;
    }
}
