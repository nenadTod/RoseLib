using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLibApp.RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Model
{
    public class AccessModifierOptions
    {
        public AccessModifierTypes AccessModifier { get; set; }

        public string ModifiersToString()
        {
            switch (AccessModifier)
            {
                case AccessModifierTypes.PRIVATE:
                    return "private";
                case AccessModifierTypes.PROTECTED:
                    return "protected";
                case AccessModifierTypes.PRIVATE_PROTECTED:
                    return "private protected";
                case AccessModifierTypes.INTERNAL:
                    return "internal";
                case AccessModifierTypes.PROTECTED_INTERNAL:
                    return "protected internal";
                case AccessModifierTypes.PUBLIC:
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
                case AccessModifierTypes.PRIVATE:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                    break;
                case AccessModifierTypes.PROTECTED:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    break;
                case AccessModifierTypes.PRIVATE_PROTECTED:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    break;
                case AccessModifierTypes.INTERNAL:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case AccessModifierTypes.PROTECTED_INTERNAL:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case AccessModifierTypes.PUBLIC:
                    retVal = retVal.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    break;
            }

            return retVal;
        }
    }
}
