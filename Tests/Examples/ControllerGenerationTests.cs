using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tests.CaseStudy
{
    public class ControllerGenerationTests
    {
        [Test]
        public void GenerateControllerBasis()
        {
            CompilationUnitComposer composer = new CompilationUnitComposer();
            var vehicleTypeControllerCode = composer
                .AddUsingDirectives(
                    "RentApp.Models.Entities",
                    "RentApp.Persistance.UnitOfWork",
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Net",
                    "System.Net.Http",
                    "System.Web.Http",
                    "System.Web.Http.Description"
                )
                .AddControllerBasis("VehicleTypesController")
                .GetCode();

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\VehicleTypeController.cs"))
            {
                writer.Write(vehicleTypeControllerCode);
            }

            Assert.Pass();
        }

        [Test]
        public void GenerateControllerBasisBasic()
        {
            CompilationUnitComposer composer = new CompilationUnitComposer();
            var vehicleTypeControllerCode = composer
                .AddUsingDirectives(
                    "RentApp.Models.Entities",
                    "RentApp.Persistance.UnitOfWork",
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Net",
                    "System.Net.Http",
                    "System.Web.Http",
                    "System.Web.Http.Description"
                )
                .AddNamespace("RentApp.Controllers")
                .EnterNamespace()
                .AddClass(new RoseLib.Model.ClassProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        ClassName = "VehicleTypeController",
                        BaseTypes = new List<string>() { "ApiController" }
                    }
                )
                .EnterClass()
                .AddField(new RoseLib.Model.FieldProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PRIVATE,
                        FieldType = "IUnitOfWork",
                        FieldName = "db"
                    }
                )
                //.AddConstructor(new RoseLib.Model.ConstructorProps()
                //  {
                //      AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                //      Params = { 
                    //    new RoseLib.Model.ParamProps()
                    //    {
                    //        Name = "context",
                    //        Type = "IUnitOfWork"
                    //    }
                    //  }
                //)
                //.EnterConstructor()
                //.EnterBody()
                //.InsertStatements("db = context;")
                .GetCode();

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\VehicleTypeController.cs"))
            {
                writer.Write(vehicleTypeControllerCode);
            }

            Assert.Pass();
        }
    }
}
