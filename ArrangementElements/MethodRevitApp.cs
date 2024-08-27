using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrangementElements
{
    internal class MethodRevitApp
    {
        UIApplication uiApp;
        UIDocument uiDoc;
        Document doc;

        public UIApplication UiApp { get => uiApp; }
        public UIDocument UiDoc { get => uiDoc; }
        public Document Doc { get => doc; }

        public MethodRevitApp(ExternalCommandData commandData)
        {
            uiApp = commandData.Application;
            uiDoc = UiApp.ActiveUIDocument;
            doc = UiDoc.Document;
        }

        public List<Level> GetListLevel()
        {
            List<Level> levels = new FilteredElementCollector(doc)
                                                .OfClass(typeof(Level))
                                                .Cast<Level>()
                                                .ToList();
            return levels;
        }

        public List<XYZ> GetPoints()
        {
            List<XYZ> points = new List<XYZ>();
            while (true)
            {
                XYZ pickedPoint = null;
                try
                {
                    pickedPoint = UiDoc.Selection.PickPoint(ObjectSnapTypes.Endpoints, "Выберете точку");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
                {
                    break;
                }
                points.Add(pickedPoint);
            }
            return points;
        }

        public List<FamilySymbol> GetFurniture()
        {
            ElementCategoryFilter elementCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Furniture);
            ElementClassFilter elementClassFilter = new ElementClassFilter(typeof(FamilySymbol));
            LogicalAndFilter logicalAndFilter = new LogicalAndFilter(elementCategoryFilter, elementClassFilter);
            List<FamilySymbol> familyInstances = new FilteredElementCollector(doc)
              .WherePasses(logicalAndFilter)
              .Cast<FamilySymbol>()
              .ToList();
            return familyInstances;
        }

        public void _CreateFurniture(List<XYZ> points, Level selectedLevel, FamilySymbol selectedFurniture)
        {
            using (var ts = new Autodesk.Revit.DB.Transaction(Doc, "Create family instance"))
            {
                ts.Start();
                if (!selectedFurniture.IsActive)
                {
                    selectedFurniture.Activate();
                    Doc.Regenerate();
                }
                for (int i = 0; i < points.Count; i++)
                {
                    Doc.Create.NewFamilyInstance(points[i], selectedFurniture, selectedLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                }
                ts.Commit();
            }
        }

    }
}
