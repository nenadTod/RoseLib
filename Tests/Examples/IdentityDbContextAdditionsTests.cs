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
    public class IdentityDbContextAdditionsTests
    {
        // PTODO: Potencijalno poboljsanje - ceo fajl iz pocetka, ako ne postoji :) Ako zatreba.
        [Test]
        public void AddDBSet()
        {
            var code = "";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\CaseStudy\\RADBContext.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                code = navigator
                    .SelectLastPropertyDeclaration()
                    .StartComposing<ClassComposer>()
                    .AddDBSet("VehicleType", "VehicleTypes")
                    .GetCode();
            }

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\RADBContext.cs"))
            {
                writer.Write(code);
            }
        }

        [Test]
        public void AddDBSetBasic()
        {
            var code = "";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\CaseStudy\\RADBContext.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                code = navigator
                    .SelectLastPropertyDeclaration()
                    .StartComposing<ClassComposer>()
                    .AddProperty(new PropertyProps()
                        {
                            AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                            PropertyType = "DbSet<VehicleType>",
                            PropertyName = "VehicleTypes"
                        }
                    )
                    .GetCode();
            }

            using (StreamWriter writer = new StreamWriter(".\\TestFiles\\CaseStudy\\RADBContext.cs"))
            {
                writer.Write(code);
            }
        }
    }
}
