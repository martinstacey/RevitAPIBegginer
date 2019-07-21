using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace SelectParameter
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Reference reference= uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            Element element = uidoc.Document.GetElement(reference);

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("transaction");
                TaskDialog.Show("tittle :)", element.Name);
                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
