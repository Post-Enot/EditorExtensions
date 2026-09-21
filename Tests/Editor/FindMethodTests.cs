using NUnit.Framework;
using PostEnot.EditorExtensions.Editor;
using System;
using System.Reflection;

/// <summary>
/// Тесты для MethodFinder.FindMethod0.
///
/// declaringType — контекст, в котором мысленно написан вызов MethodName();
/// (и статический тип получателя), targetType — runtime-тип получателя.
/// </summary>
[TestFixture]
public class FindMethodTests
{
    // =====================================================================
    //  Фикстурные типы
    // =====================================================================

    public class A
    {
        public void PublicMethod() { }
        private void PrivateMethod() { }
        protected void ProtectedMethod() { }
        internal void InternalMethod() { }

        public static void StaticMethod() { }

        public void NoArgs() { }
        public void WithArg(int x) { }

        public virtual void VirtualMethod() { }
        public void HiddenMethod() { }
    }

    public class B : A
    {
        public new void HiddenMethod() { }
        public override void VirtualMethod() { }
        public void OnlyInB() { }
    }

    public class C : B
    {
        public override void VirtualMethod() { }
    }

    // "new" вместо "override" — не участвует в virtual dispatch.
    public class V1 { public virtual void M() { } }
    public class V2 : V1 { public new void M() { } }

    // generic-метод, скрывающий не-generic базовый.
    public class G1 { public void M() { } }
    public class G2 : G1 { public void M<T>() { } }

    // Перегрузки с одним именем.
    public class Overloads
    {
        public void M() { }
        public void M(int x) { }
    }

    // private vs public с разным числом параметров.
    public class AccessMix
    {
        private void M() { }
        public void M(int x) { }
    }

    // Abstract без реализации.
    public abstract class AbstractBase { public abstract void M(); }
    public abstract class AbstractNoImpl : AbstractBase { }
    public class ConcreteImpl : AbstractBase { public override void M() { } }

    public class Unrelated { }

    // =====================================================================
    //  Обёртка над FindMethod0 (reflection, чтобы не менять production-код)
    // =====================================================================

    private static readonly MethodInfo s_find0 =
        typeof(MethodUtility).GetMethod(
            "FindMethod0",
            BindingFlags.NonPublic | BindingFlags.Static);

    private static MethodInfo Find(Type targetType, Type declaringType, string name)
    {
        Assert.That(s_find0, Is.Not.Null,
            "Не найден MethodFinder.FindMethod0 — проверьте имя типа/метода.");

        object[] args = { targetType, declaringType, name, null };
        bool found = (bool)s_find0.Invoke(null, args);
        return found ? (MethodInfo)args[3] : null;
    }

    // =====================================================================
    //  Базовые сценарии поиска в иерархии
    // =====================================================================

    [Test]
    public void Public_Inherited_FromBase()
    {
        var m = Find(typeof(B), typeof(B), nameof(A.PublicMethod));

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(A)));
    }

    [Test]
    public void Private_SameType_Found()
    {
        var m = Find(typeof(A), typeof(A), "PrivateMethod");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(A)));
        Assert.That(m.IsPrivate, Is.True);
    }

    [Test]
    public void Private_FromDerived_NotAccessible()
    {
        // Внутри B вызвать A.PrivateMethod() нельзя — метода в B нет,
        // в A он недоступен → null.
        var m = Find(typeof(B), typeof(B), "PrivateMethod");

        Assert.That(m, Is.Null);
    }

    [Test]
    public void Protected_FromDerived_Found()
    {
        var m = Find(typeof(B), typeof(B), "ProtectedMethod");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(A)));
        Assert.That(m.IsFamily, Is.True);
    }

    [Test]
    public void Protected_FromUnrelated_NotAccessible()
    {
        var m = Find(typeof(A), typeof(Unrelated), "ProtectedMethod");

        Assert.That(m, Is.Null);
    }

    // =====================================================================
    //  Hiding (new)
    // =====================================================================

    [Test]
    public void New_Hiding_DerivedWins_WhenCalledFromDerived()
    {
        var m = Find(typeof(B), typeof(B), nameof(A.HiddenMethod));

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(B)),
            "Метод в более производном типе должен скрывать базовый.");
    }

    [Test]
    public void New_Hiding_BaseWins_WhenCalledFromBase()
    {
        var m = Find(typeof(A), typeof(A), nameof(A.HiddenMethod));

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(A)));
    }

    // =====================================================================
    //  Virtual dispatch
    // =====================================================================

    [Test]
    public void Virtual_TargetIsDerived_ReturnsOverride()
    {
        // Внутри A написан вызов VirtualMethod(); получатель — B.
        var m = Find(typeof(B), typeof(A), nameof(A.VirtualMethod));

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(B)));
    }

    [Test]
    public void Virtual_Chain_ReturnsYoungestOverride()
    {
        var m = Find(typeof(C), typeof(A), nameof(A.VirtualMethod));

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(C)));
    }

    [Test]
    public void Virtual_TargetEqualsDeclaring_ReturnsBase()
    {
        var m = Find(typeof(A), typeof(A), nameof(A.VirtualMethod));

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(A)));
    }

    [Test]
    public void New_IsNotOverride_BaseVirtualReturned()
    {
        // V2.M — это new, а не override. Виртуальный диспатч не должен
        // находить V2.M при вызове M() из V1.
        var m = Find(typeof(V2), typeof(V1), "M");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(V1)),
            "new не участвует в virtual dispatch.");
    }

    // =====================================================================
    //  Перегрузки
    // =====================================================================

    [Test]
    public void Overloads_ZeroArgs_Preferred()
    {
        var m = Find(typeof(Overloads), typeof(Overloads), "M");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.GetParameters().Length, Is.EqualTo(0));
    }

    [Test]
    public void AccessMix_PrivatePreferredOverPublic()
    {
        // Документируем текущую эвристику: private > public.
        // Это НЕ C# overload resolution — без типов аргументов её и не вывести.
        var m = Find(typeof(AccessMix), typeof(AccessMix), "M");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.IsPrivate, Is.True);
    }

    // =====================================================================
    //  Static
    // =====================================================================

    [Test]
    public void Static_Found()
    {
        var m = Find(typeof(A), typeof(A), nameof(A.StaticMethod));

        Assert.That(m, Is.Not.Null);
        Assert.That(m.IsStatic, Is.True);
    }

    // =====================================================================
    //  Не найдено
    // =====================================================================

    [Test]
    public void NotFound_ReturnsNull()
    {
        var m = Find(typeof(B), typeof(B), "DoesNotExist");

        Assert.That(m, Is.Null);
    }

    // =====================================================================
    //  Известные расхождения / пограничные случаи
    // =====================================================================

    [Test]
    public void Generic_Hiding_NonGeneric_ReturnsBaseMethod()
    {
        // G2.M<T>() не может быть выведен без аргументов типа,
        // поэтому исключается из overload resolution, и вызов M()
        // внутри G2 разрешается в базовый G1.M().
        var m = Find(typeof(G2), typeof(G2), "M");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(G1)),
            "Generic-метод без вывода типов не участвует в разрешении перегрузки, "
            + "поэтому находится базовый M().");
    }

    [Test]
    public void Generic_CurrentlyFallsBackToBase()
    {
        // Фиксируем текущее поведение: возвращается базовый M(), т.к. generic
        // пропускается через ContainsGenericParameters.
        var m = Find(typeof(G2), typeof(G2), "M");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(G1)));
    }

    [Test]
    public void Abstract_WithoutOverride_ReturnsAbstractMethod()
    {
        // Документируем текущее поведение: abstract-метод возвращается как есть.
        // Вызвать его через reflection нельзя — InvalidOperationException.
        var m = Find(typeof(AbstractNoImpl), typeof(AbstractBase), "M");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.IsAbstract, Is.True);
    }

    [Test]
    public void Abstract_WithOverride_ReturnsOverride()
    {
        var m = Find(typeof(ConcreteImpl), typeof(AbstractBase), "M");

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(ConcreteImpl)));
        Assert.That(m.IsAbstract, Is.False);
    }

    [Test]
    public void TargetType_Unrelated_ReturnsBaseMethod()
    {
        // targetType не наследует declaringType — ResolveYoungestOverride
        // не должен ничего «находить» в посторонней иерархии.
        var m = Find(typeof(Unrelated), typeof(A), nameof(A.VirtualMethod));

        Assert.That(m, Is.Not.Null);
        Assert.That(m.DeclaringType, Is.EqualTo(typeof(A)),
            "targetType без связи с declaringType не влияет на диспатч.");
    }
}
