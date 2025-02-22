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
    public class ModelGenerationTests
    {
        [Test]
        public void AddModelClass()
        {
            CompilationUnitComposer composer = new CompilationUnitComposer();
            var vehicleClassCode = composer
                .AddUsingDirectives(
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Web"
                )
                .AddModelClass("VehicleTypes", "VehicleType")
                .StartNavigating()
                .SelectClassDeclaration()
                .StartComposing<ClassComposer>()
                .AddProperty(new RoseLib.Model.PropertyProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        PropertyName = "Name",
                        PropertyType = "string"
                    }
                )
                .AddProperty(new RoseLib.Model.PropertyProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        PropertyName = "Vehicles",
                        PropertyType = "List<Vehicle>"
                        // Is Virtual
                    }
                )
                .GetCode();

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\VehicleType.cs"))
            {
                writer.Write(vehicleClassCode);
            }

            NUnit.Framework.Assert.Pass();
        }


        // PTODO: Potencijalno poboljsanje - id + tip :) Ako zatreba.
        [Test]
        public void AddReferenceToModelClass()
        {
            var code = "";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\CaseStudy\\Vehicle.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                code = navigator
                    .SelectLastPropertyDeclaration()
                    .StartComposing<ClassComposer>()
                    .AddProperty(new RoseLib.Model.PropertyProps()
                        {
                            AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                            PropertyName = "VehicleType",
                            PropertyType = "VehicleType"
                            // Is Virtual
                        }
                    )
                    .GetCode();
            }

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\Vehicle.cs"))
            {
                writer.Write(code);
            }
        }


        [Test]
        public void AddModelClassBasic()
        {
            CompilationUnitComposer composer = new CompilationUnitComposer();
            var vehicleClassCode = composer
                .AddUsingDirectives(
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Web"
                )
                .AddNamespace("RentApp.Models.Entities")
                .EnterNamespace()
                .AddClass(new RoseLib.Model.ClassProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        ClassName = "VehicleType"
                    }
                )
                .EnterClass()
                .AddProperty(new RoseLib.Model.PropertyProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        PropertyType = "int",
                        PropertyName = "Id",
                        // Attribute!!!
                    }
                )
                .AddProperty(new RoseLib.Model.PropertyProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        PropertyName = "Name",
                        PropertyType = "string"
                    }
                )
                .AddProperty(new RoseLib.Model.PropertyProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        PropertyName = "Vehicles",
                        PropertyType = "List<Vehicle>"
                        // Is Virtual!!!
                    }
                )
                .GetCode();

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\VehicleType.cs"))
            {
                writer.Write(vehicleClassCode);
            }

            Assert.Pass();
        }
    }
}
