using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace RoseLib.Model
{
    

    
    public class AccessModifierProps
    {
        public AccessModifiers AccessModifier { get; set; }

        public string ModifiersToString()
        {
            switch (AccessModifier)
            {
                case AccessModifiers.PRIVATE:
                    return "private";
                case AccessModifiers.PROTECTED:
                    return "protected";
                case AccessModifiers.PRIVATE_PROTECTED:
                    return "private protected";
                case AccessModifiers.INTERNAL:
                    return "internal";
                case AccessModifiers.PROTECTED_INTERNAL:
                    return "protected internal";
                case AccessModifiers.PUBLIC:
                    return "public";
                default:
                    return "";
            }
        }

        public SyntaxTokenList ModifiersToTokenList()
        {
            SyntaxTokenList retVal = new SyntaxTokenList();
            switch (AccessModifier)
            {
                case AccessModifiers.PRIVATE:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                    break;
                case AccessModifiers.PROTECTED:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    break;
                case AccessModifiers.PRIVATE_PROTECTED:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    break;
                case AccessModifiers.INTERNAL:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case AccessModifiers.PROTECTED_INTERNAL:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case AccessModifiers.PUBLIC:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    break;
            }

            return retVal;
        }
    }
}