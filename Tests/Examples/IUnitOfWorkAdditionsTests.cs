using Microsoft.CodeAnalysis.CSharp;
using RoseLib.Composers;
using RoseLib.Exceptions;
using RoseLib.Model;
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
    public class IUnitOfWorkAdditionsTests
    {
        [Test]
        public void AddIRepositoryBasic()
        {
            var code = "";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\CaseStudy\\IUnitOfWork.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                code = navigator
                    .SelectLastPropertyDeclaration()
                    .StartComposing<InterfaceComposer>()
                    .AddProperty(
                        new PropertyProps()
                        {
                            PropertyType = "IVehicleType",
                            PropertyName = "VehicleTypes"
                        }
                    )
                    .GetCode();
            }

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\IUnitOfWork.cs"))
            {
                writer.Write(code);
            }
        }
    }
}
