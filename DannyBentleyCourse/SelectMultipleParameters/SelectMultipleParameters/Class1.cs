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
            Reference reference = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            Element element = uidoc.Document.GetElement(reference);

            IList<Reference> pickedObjs = uidoc.Selection.PickObjects(ObjectType.Element, "Select elements"); ;
            List<ElementId> ids =(from Reference r in pickedObjs select r.ElementId).ToList();
            using (Transaction tx = new Transaction(doc))
            {
                StringBuilder sb = new StringBuilder();
                tx.Start("transaction");
                if (pickedObjs!= null && pickedObjs.Count > 0)
                {
                    foreach (ElementId eid in ids)
                    {
                        Element e = doc.GetElement(eid);
                        sb.Append("\n" + e.Name);
                    }
                    TaskDialog.Show("tittle :)", sb.ToString());
                }                
                tx.Commit();
            }
            return Result.Succeeded;
        }
    }
}