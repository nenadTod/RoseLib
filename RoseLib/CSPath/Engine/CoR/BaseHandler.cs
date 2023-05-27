using RoseLib.CSPath.Exceptions;
using RoseLib.CSPath.Model;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RoseLib.CSPath.Engine.CoR
{
    internal abstract class BaseHandler
    {
        protected Dictionary<Concept, MethodInfo> conceptHandlingMethods = new Dictionary<Concept, MethodInfo>();

        internal BaseHandler? NextHandler { get; set; }
        internal abstract void HandleDescent(Context context);
        protected void HandleDescendForNavigator<T>(Context context) where T : BaseNavigator
        {
            if (context == null) { throw new ArgumentNullException("context"); }

            if (!(context.Visitor is T))
            {
                if (NextHandler != null) { NextHandler.HandleDescent(context); }
                else { throw new PathNotSupportedExeption(context.PathPart); }
            }
            else
            {
                Descend(context.Visitor, typeof(T), context);
            }
        }
        protected virtual void Descend(object visitor, Type visitorType, Context context)
        {

            var concept = context.PathPart.Concept;

            var methodInfo = conceptHandlingMethods[concept];
            var specializedMethodInfo = methodInfo.MakeGenericMethod(visitorType);


            if (concept.Predicate == null)
            {
                context.Visitor = (specializedMethodInfo.Invoke(visitor, new object[] { visitor }) as IStatefulVisitor)!;
            }
            else
            {
                context.Visitor = (specializedMethodInfo.Invoke(visitor, new object[] { visitor, concept.Predicate.Value! }) as IStatefulVisitor)!;
            }
        }
        protected virtual void InitializeForType(Type type) 
        {
            Assembly thisAssembly = typeof(BaseHandler).Assembly;
            var extensionMethods = GetExtensionMethods(thisAssembly, type);
            ClassifyExtensionMethods(extensionMethods);
        }

        private void ClassifyExtensionMethods(IEnumerable<MethodInfo> extensionMethods)
        {
            foreach (MethodInfo method in extensionMethods)
            {
                var cSPathConfig = method.GetCustomAttribute(typeof(CSPathConfigAttribute)) as CSPathConfigAttribute;
                if(cSPathConfig != null)
                {
                    var concept = cSPathConfig.ToCSPathModel();
                    conceptHandlingMethods.Add(concept, method);
                }
            }
        }

        private IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {
            var isGenericTypeDefinition = extendedType.IsGenericType && extendedType.IsTypeDefinition;

            var candidateTypes = assembly.GetTypes()
                .Where(type => !type.IsGenericType && !type.IsNested)
                .ToList();

            List<MethodInfo> candidateMethods = new List<MethodInfo>();
            foreach (var candidateType in candidateTypes)
            {
                var filteredTypeMethods = candidateType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(method => method.IsDefined(typeof(ExtensionAttribute), false));
                candidateMethods.AddRange(filteredTypeMethods);
            }

            List<MethodInfo> resultingMethods = new List<MethodInfo>();
            foreach (var candidateMethod in candidateMethods)
            {
                if(candidateMethod.GetParameters()[0].ParameterType == extendedType)
                {
                    resultingMethods.Add(candidateMethod);
                }
                else
                {
                    Type genericParam0 = candidateMethod.GetParameters()[0].ParameterType;
                    if(!genericParam0.IsGenericMethodParameter) { continue; }

                    Type[] constraints = genericParam0.GetGenericParameterConstraints();
                    foreach (var constraint in constraints)
                    {
                        if(TypeInheritsFrom(extendedType, constraint))
                        {
                            resultingMethods.Add(candidateMethod);
                        }
                    }
                }
            }


            return resultingMethods;
        }

        private static bool TypeInheritsFrom(Type type, Type baseType)
        {
            // null does not have base type
            if (type == null)
            {
                return false;
            }

            // only interface or object can have null base type
            if (baseType == null)
            {
                return type.IsInterface || type == typeof(object);
            }

            // check implemented interfaces
            if (baseType.IsInterface)
            {
                return type.GetInterfaces().Contains(baseType);
            }

            // check all base types
            var currentType = type;
            while (currentType != null)
            {
                if (currentType.BaseType == baseType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }
    }
}
