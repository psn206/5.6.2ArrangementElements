using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrangementElements
{
    internal class MainViewViewModel
    {

        MethodRevitApp methodRevitApp;
        public DelegateCommand SaveCommand { get; }
        public List<Level> Levels { get; set; } = new List<Level>();
        public Level SelectedLevel { get; set; }
        List<XYZ> Points { get; } = new List<XYZ>();
        public List<FamilySymbol> Furniture { get; set; } = new List<FamilySymbol>();
        public FamilySymbol SelectedFurniture { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            SaveCommand = new DelegateCommand(OnSaveCommand);
            methodRevitApp = new MethodRevitApp(commandData);
            Levels = methodRevitApp.GetListLevel();
            Points = methodRevitApp.GetPoints();
            Furniture = methodRevitApp.GetFurniture();
        }

        private void OnSaveCommand()
        {
            methodRevitApp._CreateFurniture(Points, SelectedLevel, SelectedFurniture);
            RaiseCloseRecuest();
        }

        public event EventHandler CloseRecuest;

        public void RaiseCloseRecuest()
        {
            CloseRecuest?.Invoke(this, EventArgs.Empty);
        }
    }
}
