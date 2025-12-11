namespace SportSynchro.Domain.Exceptions;

public sealed class LeagueException(string message) : DomainException(message);