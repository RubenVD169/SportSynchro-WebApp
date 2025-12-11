namespace SportSynchro.Domain.Exceptions;

public sealed class MatchException(string message) : DomainException(message);
