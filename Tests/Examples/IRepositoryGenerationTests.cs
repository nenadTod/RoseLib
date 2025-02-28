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
    public class IRepositoryGenerationTests
    {
        [Test]
        public void AddIRepositoryBasis()
        {
            CompilationUnitComposer composer = new CompilationUnitComposer();
            var vehicleClassCode = composer
                .AddUsingDirectives(
                    "RentApp.Models.Entities",
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Text",
                    "System.Threading.Tasks"
                )
                .AddIRepositoryBasis("VehicleTypeRepository", "VehicleType")
                .EnterNamespace()
                .EnterInterface()
                .AddIRepositoryGetAllMethod()
                .GetCode();

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\IVehicleTypeRepository.cs"))
            {
                writer.Write(vehicleClassCode);
            }

            Assert.Pass();
        }


        [Test]
        public void AddIRepositoryBasisBasic()
        {
            CompilationUnitComposer composer = new CompilationUnitComposer();
            var vehicleClassCode = composer
                .AddUsingDirectives(
                    "RentApp.Models.Entities",
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Text",
                    "System.Threading.Tasks"
                )
                .AddNamespace("RentApp.Persistance.Repository")
                .EnterNamespace()
                .AddInterface(new RoseLib.Model.InterfaceProps()
                    {
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        InterfaceName = "IVehicleTypeRepository",
                        BaseTypes = new List<string>() { "IRepository<VehicleType,int>" }
                    }
                )
                .EnterInterface()
                .AddMethod(new RoseLib.Model.MethodProps()
                    {
                        ReturnType = "IEnumerable<VehicleType>",
                        MethodName = "GetAll",
                        Params = { 
                            new RoseLib.Model.ParamProps() {
                                Name = "pageIndex",
                                Type = "int"
                            },
                            new RoseLib.Model.ParamProps() {
                                Name = "pageSize",
                                Type = "int"
                            }
                        },
                        BodylessMethod = true
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
