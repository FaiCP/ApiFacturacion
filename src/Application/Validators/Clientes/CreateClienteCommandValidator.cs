using Application.Commands.Clientes;
using Domain.Enums;
using Domain.Validators;
using FluentValidation;

namespace Application.Validators.Clientes;

public class CreateClienteCommandValidator : AbstractValidator<CreateClienteCommand>
{
    public CreateClienteCommandValidator()
    {
        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social es obligatoria.")
            .MaximumLength(300).WithMessage("La razón social no puede exceder 300 caracteres.");

        RuleFor(x => x.NumeroIdentificacion)
            .NotEmpty().WithMessage("El número de identificación es obligatorio.")
            .Must((cmd, numero) => ValidarIdentificacion(cmd.TipoIdentificacion, numero))
            .WithMessage("El número de identificación no es válido para el tipo seleccionado.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }

    private static bool ValidarIdentificacion(TipoIdentificacion tipo, string numero) =>
        tipo switch
        {
            TipoIdentificacion.Cedula => CedulaValidator.EsValida(numero),
            TipoIdentificacion.Ruc => RucValidator.EsValido(numero),
            TipoIdentificacion.Pasaporte => !string.IsNullOrWhiteSpace(numero) && numero.Length <= 20,
            TipoIdentificacion.ConsumidorFinal => numero == "9999999999999",
            _ => false
        };
}
