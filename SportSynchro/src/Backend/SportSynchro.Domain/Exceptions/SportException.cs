namespace SportSynchro.Domain.Exceptions;

public sealed class SportException(string message) : DomainException(message);
