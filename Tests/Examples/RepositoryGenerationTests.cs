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
    public class RepositoryGenerationTests
    {
        [Test]
        public void AddGetAllToRepository()
        {
            CompilationUnitComposer composer = new CompilationUnitComposer();
            var vehicleRepositoryCode = composer
                .AddUsingDirectives(
                    "RentApp.Models.Entities",
                    "System.Collections.Generic",
                    "System.Data.Entity",
                    "System.Linq"
                )
                .AddRepositoryClass("VehicleTypeRepository", "VehicleType", "IVehicleTypeRepository")
                .StartNavigating()
                .SelectClassDeclaration()
                .StartComposing<ClassComposer>()
                .AddGetAllRepositoryMethod("VehicleType", "VehicleTypes")
                .GetCode();

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\VehicleTypeRepository.cs"))
            {
                writer.Write(vehicleRepositoryCode);
            }

            Assert.Pass();
        }

        [Test]
        public void AddRepositoryBasic()
        {
            CompilationUnitComposer composer = new CompilationUnitComposer();
            var vehicleClassCode = composer
                .AddUsingDirectives(
                    "RentApp.Models.Entities",
                    "System.Collections.Generic",
                    "System.Data.Entity",
                    "System.Linq"
                )
                .AddNamespace("RentApp.Persistance.Repository")
                .EnterNamespace()
                .AddClass(new RoseLib.Model.ClassProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        ClassName = "VehicleTypeRepository",
                        BaseTypes = new List<string>()
                        {
                            "Repository<VehicleType, int>",
                            "IVehicleTypeRepository"
                        }
                    }
                )
                .EnterClass()
                .AddProperty(new RoseLib.Model.PropertyProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PROTECTED,
                        PropertyType = "RADBContext",
                        PropertyName = "RADBContext",
                    }
                )
                //.EnterProperty()
                //.EnterGetBody()
                //.InsertStatements("return context as RADBContext;")
                //.StartNavigating()
                //.SelectProperty("RADBContext")
                //.StartComposing<ClassComposer>()
                //.AddConstructor(new RoseLib.Model.ConstructorProps()
                //  {
                //      AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                //      Params = { 
                        //    new RoseLib.Model.ParamProps()
                        //    {
                        //        Name = "pageIndex",
                        //        Type = "int"
                        //    },
                        //    new RoseLib.Model.ParamProps()
                        //    {
                        //        Name = "pageSize",
                        //        Type = "int"
                        //    }
                        //},
                //      BaseParams = {  
                //            new RoseLib.Model.ParamProps()
                        //    {
                        //        Name = "pageSize",
                        //    }
                //      }
                //  }
                //)
                .AddMethod(new RoseLib.Model.MethodProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        ReturnType = "IEnumerable<BranchOffice>",
                        MethodName = "GetAll",
                        Params = new List<RoseLib.Model.ParamProps>()
                        {
                            new RoseLib.Model.ParamProps()
                            {
                                Type = "int",
                                Name = "pageIndex"
                            },
                            new RoseLib.Model.ParamProps()
                            {
                                Type = "int",
                                Name = "pageSize"
                            }
                        }
                    }
                )
                .EnterMethod()
                .EnterBody()
                .InsertStatements("return RADBContext.VehicleTypes.Skip((pageIndex - 1) * pageSize).Take(pageSize);")
                .GetCode();

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\VehicleType.cs"))
            {
                writer.Write(vehicleClassCode);
            }

            Assert.Pass();
        }
    }
}
