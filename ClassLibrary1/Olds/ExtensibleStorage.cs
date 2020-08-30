using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    class ExtensibleStorage : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //  Get the access to the top most objects. 
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Transaction trans = new Transaction(doc, "Extensible Storage");
            trans.Start();

            // Pick a wall
            Reference r = uiDoc.Selection.PickObject(ObjectType.Element, new WallSelectionFilter());
            Wall wall = doc.GetElement(r) as Wall;

            Guid _guid = new Guid("87aaad89-6f1b-45e1-9397-2985e1560a02");

            // Create a schema builder
            SchemaBuilder builder = new SchemaBuilder(_guid);

            // Set read and write access levels
            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Public);

            // Note: if this was set as vendor or application access, 
            // we would have been additionally required to use SetVendorId

            // Set name to this schema builder
            builder.SetSchemaName("WallSocketLocation");
            builder.SetDocumentation("Data store for socket info in a wall");

            // Create field1
            FieldBuilder fieldBuilder1 = builder.AddSimpleField("SocketLocation", typeof(XYZ));

            // Set unit type
            fieldBuilder1.SetUnitType(UnitType.UT_Length);

            // Add documentation (optional)

            // Create field2
            FieldBuilder fieldBuilder2 = builder.AddSimpleField("SocketNumber", typeof(string));

            //fieldBuilder2.SetUnitType(UnitType.UT_Custom);

            // Register the schema object
            Schema schema = builder.Finish();

            // Create an entity (object) for this schema (class)

            Entity ent = new Entity(schema);

            Field socketLocation = schema.GetField("SocketLocation");
            ent.Set<XYZ>(socketLocation, new XYZ(2, 0, 0), DisplayUnitType.DUT_METERS);

            Field socketNumber = schema.GetField("SocketNumber");
            ent.Set(socketNumber, "200");

            wall.SetEntity(ent);

            // Now create another entity (object) for this schema (class)
            // (This simply replaces the ent1 above. Just for testing.
            //  You may comment out for now.) 

            Entity ent2 = new Entity(schema);
            Field socketNumber1 = schema.GetField("SocketNumber");
            ent2.Set<String>(socketNumber1, "400");
            wall.SetEntity(ent2);

            // List all schemas in the document

            string s = string.Empty;
            IList<Schema> schemas = Schema.ListSchemas();
            foreach (Schema sch in schemas)
            {
                s += "\r\nSchema Name: " + sch.SchemaName;
            }
            TaskDialog.Show("Schema details", s);

            // List all Fields for our schema
            s = string.Empty;
            Schema ourSchema = Schema.Lookup(_guid);
            IList<Field> fields = ourSchema.ListFields();
            foreach (Field fld in fields)
            {
                s += "\r\nField Name: " + fld.FieldName;
            }
            TaskDialog.Show("Field details", s);

            // Extract the value for the field we created

            Entity wallSchemaEnt = wall.GetEntity(Schema.Lookup(_guid));

            XYZ wallSocketPos = wallSchemaEnt.Get<XYZ>(Schema.Lookup(_guid).GetField("SocketLocation"), DisplayUnitType.DUT_METERS);

            s = "SocketLocation: " + DBElement.PointToString(wallSocketPos);

            string wallSocketNumber = wallSchemaEnt.Get<String>(Schema.Lookup(_guid).GetField("SocketNumber"));

            s += "\r\nSocketNumber: " + wallSocketNumber;
            TaskDialog.Show("Field values", s);


            trans.Commit();

            return Result.Succeeded;
        }
    }
    class WallSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e) => e is Wall;

        public bool AllowReference(Reference r, XYZ p) => true;        
    }
}
