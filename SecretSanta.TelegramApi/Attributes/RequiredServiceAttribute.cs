namespace SecretSanta.TelegramApi.Attributes;

public enum InjectionType
{
    Singleton,
    Scoped,
    Transient
}

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class RequiredServiceAttribute(InjectionType injectionType) : Attribute
{
    public InjectionType InjectionType { get; } = injectionType;
}
