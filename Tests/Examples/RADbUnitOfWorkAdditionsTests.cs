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
    public class RADbUnitOfWorkAdditionsTests
    {
        // PTODO: Potencijalno poboljsanje - ceo fajl iz pocetka, ako ne postoji :) Ako zatreba.
        [Test]
        public void AddIRepository()
        {
            var code = "";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\CaseStudy\\RADBUnitOfWork.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                code = navigator
                    .SelectLastPropertyDeclaration()
                    .StartComposing<ClassComposer>()
                    .AddUoWDependency("IVehicleTypeRepository", "VehicleTypes")
                    .GetCode();
            }

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\RADBUnitOfWork.cs"))
            {
                writer.Write(code);
            }
        }

        [Test]
        public void AddIRepositoryBasic()
        {
            var code = "";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\CaseStudy\\RADBUnitOfWork.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                code = navigator
                    .SelectLastPropertyDeclaration()
                    .StartComposing<ClassComposer>()
                    .AddProperty(
                        new PropertyProps()
                        {
                            // Attribute List
                            AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                            PropertyType = "IVehicleType",
                            PropertyName = "VehicleTypes"
                        }
                    )
                    .GetCode();
            }

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\RADBUnitOfWork.cs"))
            {
                writer.Write(code);
            }
        }


    }
}
