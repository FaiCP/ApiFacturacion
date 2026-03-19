namespace Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string name, object key)
        : base($"Entidad '{name}' con identificador '{key}' no fue encontrada.") { }

    public NotFoundException(string message) : base(message) { }
}
