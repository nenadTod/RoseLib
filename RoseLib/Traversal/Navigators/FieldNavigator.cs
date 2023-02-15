﻿using RoseLib.Composers;
using RoseLib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class FieldNavigator: BaseNavigator
    {
        internal FieldNavigator(BaseNavigator? parentNavigator): base(parentNavigator)
        {
        }

        public T StartComposing<T>() where T: BaseComposer
        {
            // TODO: Extend for different kinds of possible composers.
            if(typeof(T).Equals(typeof(ClassComposer)))
            {
                return (new ClassComposer(this) as T)!;
            }
            else
            {
                throw new Exceptions.InvalidCastException();
            }
        }
    }
}
