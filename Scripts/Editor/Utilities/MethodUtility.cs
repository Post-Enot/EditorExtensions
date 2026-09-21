#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PostEnot.EditorExtensions.Editor
{
    public static class MethodUtility
    {
        internal static bool TryFindMethod(
            Type? targetType,
            Type? declaringType,
            string? methodName,
            [NotNullWhen(true)] out MethodInfo? methodInfo)
        {
            if (targetType == null)
            {
                methodInfo = null;
                return false;
            }
            if (string.IsNullOrWhiteSpace(methodName))
            {
                methodInfo = null;
                return false;
            }
            if (declaringType == null)
            {
                methodInfo = null;
                return false;
            }
            if (!declaringType.IsAssignableFrom(targetType))
            {
                methodInfo = null;
                return false;
            }
            ReadOnlySpan<char> methodNameSpan = methodName.AsSpan().Trim();
            int dotIndex = methodNameSpan.LastIndexOf('.');
            string methodNameTrimmed = methodNameSpan.ToString();
            if (methodNameTrimmed == string.Empty)
            {
                methodInfo = null;
                return false;
            }
            if (dotIndex < 0)
            {
                return FindMethod0(targetType, declaringType, methodNameTrimmed, out methodInfo);
            }
            methodInfo = null;
            return false;
        }

        internal static bool FindMethod0(
            Type targetType,
            Type declaringType,
            string methodName,
            [NotNullWhen(true)] out MethodInfo? methodInfo)
        {
            // Идём от declaringType вверх по базовым типам: метод в более
            // производном типе скрывает одноимённые методы базовых типов.
            for (Type? current = declaringType; current != null; current = current.BaseType)
            {
                MethodInfo? found = FindInType(current, methodName, declaringType);
                if (found != null)
                {
                    methodInfo = ResolveYoungestOverride(found, targetType);
                    return true;
                }
            }
            methodInfo = null;
            return false;
        }

        private static MethodInfo? FindInType(Type type, string methodName, Type accessingType)
        {
            const BindingFlags flags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly;
            MethodInfo?[] candidates = type.GetMethods(flags);
            MethodInfo? best = null;
            foreach (MethodInfo? candidate in candidates)
            {
                if (candidate == null)
                {
                    continue;
                }
                if (candidate.Name != methodName)
                {
                    continue;
                }

                // Открытые generic-методы пропускаем: у нас нет аргументов типа,
                // чтобы их замкнуть.
                if (candidate.IsGenericMethodDefinition)
                {
                    continue;
                }
                if (!IsAccessibleFrom(candidate, accessingType))
                {
                    continue;
                }

                if (IsBetterMethod(candidate, best))
                {
                    best = candidate;
                }
            }
            return best;
        }

        /// <summary>
        /// Для virtual/abstract-метода возвращает самый производный override
        /// в цепочке от <paramref name="fromType"/> вниз до <see cref="MemberInfo.DeclaringType"/>.
        /// </summary>
        private static MethodInfo ResolveYoungestOverride(MethodInfo method, Type fromType)
        {
            if (!method.IsVirtual)
            {
                return method;
            }
            MethodInfo baseDefinition = method.GetBaseDefinition();
            for (Type? current = fromType;
                 current != null && current != method.DeclaringType;
                 current = current.BaseType)
            {
                foreach (MethodInfo m in current.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly))
                {
                    if (m.Name != method.Name)
                        continue;

                    MethodInfo mBase = m.GetBaseDefinition();

                    // Сравниваем по Module + MetadataToken: GetBaseDefinition()
                    // может возвращать разные экземпляры MethodInfo для одного и
                    // того же метода.
                    if (mBase.Module == baseDefinition.Module &&
                        mBase.MetadataToken == baseDefinition.MetadataToken)
                    {
                        return m;
                    }
                }
            }
            return method;
        }

        /// <summary>
        /// Эвристика выбора «лучшего» кандидата среди одноимённых методов.
        /// Типы аргументов отсутствуют, поэтому строго разрешить перегрузку
        /// нельзя — правила носят договорной характер.
        /// </summary>
        private static bool IsBetterMethod(MethodInfo candidate, MethodInfo current)
        {
            if (current == null)
            {
                return true;
            }
            // 1. Приоритет доступа (меньше значение — выше приоритет).
            int candidateAccess = GetAccessPriority(candidate);
            int currentAccess = GetAccessPriority(current);
            if (candidateAccess != currentAccess)
            {
                return candidateAccess < currentAccess;
            }

            // 2. Статика > инстанс.
            if (candidate.IsStatic != current.IsStatic)
            {
                return candidate.IsStatic;
            }

            // 3. void > не-void.
            bool candidateVoid = candidate.ReturnType == typeof(void);
            bool currentVoid = current.ReturnType == typeof(void);
            if (candidateVoid != currentVoid)
            {
                return candidateVoid;
            }

            // 4. Меньше параметров — лучше.
            int candidateParams = candidate.GetParameters().Length;
            int currentParams = current.GetParameters().Length;
            if (candidateParams != currentParams)
            {
                return candidateParams < currentParams;
            }
            return false;
        }

        private static int GetAccessPriority(MethodInfo method)
        {
            if (method.IsPrivate) return 0;                // private
            if (method.IsFamilyAndAssembly) return 1;      // private protected
            if (method.IsFamilyOrAssembly) return 2;       // protected internal
            if (method.IsFamily) return 3;                 // protected
            if (method.IsAssembly) return 4;               // internal
            if (method.IsPublic) return 5;                 // public
            return int.MaxValue;
        }

        /// <summary>
        /// Можем ли мы вызвать <paramref name="method"/> из контекста
        /// <paramref name="accessingType"/>.
        /// </summary>
        private static bool IsAccessibleFrom(MethodInfo method, Type accessingType)
        {
            if (method.IsPublic)
            {
                return true;
            }

            Type? declaringType = method.DeclaringType;
            if (declaringType == null)
            {
                return false;
            }

            bool sameAssembly = declaringType.Assembly == accessingType.Assembly;
            bool isDerived = declaringType.IsAssignableFrom(accessingType);

            if (method.IsPrivate)              // private
                return declaringType == accessingType;
            if (method.IsAssembly)             // internal
                return sameAssembly;
            if (method.IsFamily)               // protected
                return isDerived;
            if (method.IsFamilyOrAssembly)     // protected internal
                return isDerived || sameAssembly;
            if (method.IsFamilyAndAssembly)    // private protected
                return isDerived && sameAssembly;
            return false;
        }
    }
}
