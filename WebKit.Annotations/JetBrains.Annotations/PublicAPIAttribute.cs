// ReSharper disable CheckNamespace

namespace JetBrains.Annotations;

/// <summary>
/// </summary>
[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)] [PublicAPI]
[AttributeUsage(AttributeTargets.All)]
// ReSharper disable once InconsistentNaming
internal sealed class PublicAPIAttribute : Attribute;
