namespace SecretSantaTgBot.Attributes;

public enum InjectionType
{
    Singleton,
    Scoped,
    Transient
}

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class RequiredServiceAttribute(InjectionType injectionType = InjectionType.Singleton) : Attribute
{
    public InjectionType InjectionType { get; init; } = injectionType;
}
