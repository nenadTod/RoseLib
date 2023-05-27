using RoseLib.CSPath;
using RoseLib.CSPath.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Extensions
{
    internal static class ExtensionsHelper
    {
        internal static PathPart GetPathPartForMethod(MethodBase method)
        {
            CSPathConfigAttribute configAttribute = (CSPathConfigAttribute)method.GetCustomAttribute(typeof(CSPathConfigAttribute))!;
            if( configAttribute == null) { throw new Exception("Provided method does not have CSPath configuration attribute."); }
            if (configAttribute.Concept == null) { throw new Exception("Concept not set through config attribute"); }

            var descend = new Descend("/");
            var concept = new Concept(configAttribute.Concept, null);

            return new PathPart(descend, concept);
        }

        internal static PathPart GetPathPartForMethodAndValue(MethodBase method, string value)
        {
            CSPathConfigAttribute configAttribute = (CSPathConfigAttribute)method.GetCustomAttribute(typeof(CSPathConfigAttribute))!;
            if (configAttribute == null) { throw new Exception("Provided method does not have CSPath configuration attribute."); }
            if(configAttribute.Concept == null) { throw new Exception("Concept not set through config attribute"); }

            var descend = new Descend("/");

            if(configAttribute.Attribute == null) { throw new Exception("Attribute is necessary for creating path part with a value"); }
            var predicate = new Predicate(configAttribute.Attribute, value);
            var concept = new Concept(configAttribute.Concept, predicate);


            return new PathPart(descend, concept);
        }
    }
}
