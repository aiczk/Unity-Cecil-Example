﻿using System;

namespace Mono_Cecil_Sample.Attributes
{
    [AttributeUsage
        (
            AttributeTargets.Class |
            AttributeTargets.Enum |
            AttributeTargets.Interface |
            AttributeTargets.Struct | 
            AttributeTargets.GenericParameter
        )
    ]
    public class ModAttribute : Attribute
    {
    }
}
