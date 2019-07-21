using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace Lab1PlaceGroup
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;                            //Get application and document objects
            Document doc = uiapp.ActiveUIDocument.Document;
            Reference pickedref = null;                                               //Define a reference Object to accept the pick result
            Selection sel = uiapp.ActiveUIDocument.Selection;                         //Pick a group
            pickedref = sel.PickObject(ObjectType.Element, "Please select a group");
            Element elem = doc.GetElement(pickedref);
            Group group = elem as Group;
            XYZ point = sel.PickPoint("Please pick a point to place group");          //Pick point
            Transaction trans = new Transaction(doc);                                 //Place the group
            trans.Start("Lab");
            doc.Create.PlaceGroup(point, group.GroupType);
            trans.Commit();
            return Result.Succeeded;
        }
    }
}