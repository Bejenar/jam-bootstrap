using System;
using System.Linq;
using System.Reflection;

public static class ReflectionUtil
{
    public static Type[] FindAllSubslasses<T>(Type rootType)
    {
        Type baseType = typeof(T);
        Assembly assembly = Assembly.GetAssembly(baseType);
        Assembly rootAssembly = Assembly.GetAssembly(rootType);
        
        Type[] types = assembly.GetTypes().Concat(rootAssembly.GetTypes()).ToArray();
        Type[] subclasses = types.Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract).ToArray();

        return subclasses;
    }
}