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
using Autodesk.Revit.DB.Architecture;

namespace Lab1PlaceGroup
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            try
            {
                Reference pickedRef = null;    //Define a reference Object to accept the pick result
                Selection sel = uiapp.ActiveUIDocument.Selection;      //Pick a group
                GroupPickFilter selFilter = new GroupPickFilter();
                pickedRef = sel.PickObject(ObjectType.Element, selFilter, "Please select a group");
                Element elem = doc.GetElement(pickedRef);
                Group group = elem as Group;
                XYZ origin = GetElementCenter(group);      //// Get the group's center point
                Room room = GetRoomOfGroup(doc, origin);                   // Get the room that the picked group is located in
                XYZ sourceCenter = GetRoomCenter(room);                  // Get the room's center point
                string coords = "X = " + sourceCenter.X.ToString() + "\r\n" + "Y = " + sourceCenter.Y.ToString() + "\r\n" + "Z = " + sourceCenter.Z.ToString();
                TaskDialog.Show("Source room Center", coords);       
                Transaction trans = new Transaction(doc);         //Place the group
                trans.Start("Lab");
                XYZ groupLocation = sourceCenter + new XYZ(20, 0, 0);    // Calculate the new group's position
                doc.Create.PlaceGroup(groupLocation, group.GroupType);
                trans.Commit();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
           
            return Result.Succeeded;
        }
        public class GroupPickFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                return (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_IOSModelGroups));
            }
            public bool AllowReference(Reference r, XYZ p)
            {
                return false;
            }

        }
        public XYZ GetElementCenter(Element elem)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);
            XYZ center = (bounding.Max + bounding.Min) * 0.5;
            return center;
        }
        Room GetRoomOfGroup(Document doc, XYZ point)
        {
            FilteredElementCollector collector =new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Rooms);
            Room room = null;
            foreach (Element elem in collector)
            {
                room = elem as Room;
                if (room != null)
                {
                    if (room.IsPointInRoom(point))   // Decide if this point is in the picked room  
                    {
                        break;
                    }                   
                }
            }
            return room;
        }    /// Return a room's center point coordinates.    /// Z value is equal to the bottom of the room
        public XYZ GetRoomCenter(Room room)
        {
            XYZ boundCenter = GetElementCenter(room);         // Get the room center point.
            LocationPoint locPt = (LocationPoint)room.Location;
            XYZ roomCenter = new XYZ(boundCenter.X, boundCenter.Y, locPt.Point.Z);
            return roomCenter;
        }
    }
}

