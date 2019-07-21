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
    public class Class1 : IExternalCommand
    {        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;   //Get application and document objects
            Document doc = uiapp.ActiveUIDocument.Document;
            try
            {
                Reference pickedRef = null;                           //Define a reference Object to accept the pick result
                Selection sel = uiapp.ActiveUIDocument.Selection;     //Pick a group
                GroupPickFilter selFilter = new GroupPickFilter();
                pickedRef = sel.PickObject(ObjectType.Element, selFilter, "Please select a group");
                Element elem = doc.GetElement(pickedRef);
                Group group = elem as Group;
                XYZ point = sel.PickPoint("Please pick a point to place group");  //Pick point
                Transaction trans = new Transaction(doc);                         //Place the group
                trans.Start("Lab");
                doc.Create.PlaceGroup(point, group.GroupType);
                trans.Commit();
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)       //If the user right-clicks or presses Esc, handle the exception
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        public class GroupPickFilter : ISelectionFilter     // Filter to constrain picking to model groups. Only model groups
        {                                                    // are highlighted and can be selected when cursor is hovering.
            public bool AllowElement(Element e)
            {
                return (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_IOSModelGroups));
            }
            public bool AllowReference(Reference r, XYZ p)
            {
                return false;
            }
        }
    }
   
}