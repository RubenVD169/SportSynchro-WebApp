namespace SportSynchro.Domain.Exceptions;

public sealed class TeamException(string message) : DomainException(message);
