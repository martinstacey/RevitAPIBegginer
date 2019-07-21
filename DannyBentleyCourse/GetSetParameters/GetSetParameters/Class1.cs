using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using System.Text;


namespace GetSetParameters
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Element e = SelectElement(uidoc, doc);
            Parameter parameter = e.LookupParameter("Comment");

            using (Transaction t = new Transaction(doc,"parameter"))
            {
                t.Start("param");
                try
                {
                    parameter.Set("test comment");
                }
                catch { }
                t.Commit();
            }

            return Result.Succeeded;
        }
        public Element SelectElement (UIDocument uidoc, Document doc)
        {
            Reference reference = uidoc.Selection.PickObject(ObjectType.Element);
            Element element = uidoc.Document.GetElement(reference);
            return element;
        }
    }
}
