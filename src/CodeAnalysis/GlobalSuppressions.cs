// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

// Common rule suppressions for all projects

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1000:Do not declare static members on generic types",
    Justification = "Not a problem usually",
    Scope = "module")]

[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords",
    Justification = "Not a problem usually",
    Scope = "module")]

[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
    Justification = "Not all projects should be localized",
    Scope = "module")]

[assembly: SuppressMessage("Naming", "CA1720:Identifier contains type name",
    Justification = "Alarms for absolutely safe obj, whre it's appropriate. Discussion in progress.",
    Scope = "module")]

[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays",
    Justification = "Not a problem usually if developers are qualified",
    Scope = "module")]

[assembly: SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings",
    Justification = "Too strong requirement",
    Scope = "module")]

[assembly: SuppressMessage("Style", "IDE0022:Use expression body for methods",
    Justification = "Too strong requirement",
    Scope = "module")]

[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement",
    Justification = "Simetimes it's useful for code clarity",
    Scope = "module")]

[assembly: SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task",
    Justification = "That's not necessary in ASP.NET Core, which doesn't set up an own SynchronizationContext",
    Scope = "module")]
