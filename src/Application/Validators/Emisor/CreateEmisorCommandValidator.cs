using Application.Commands.Emisor;
using Domain.Validators;
using FluentValidation;

namespace Application.Validators.Emisor;

public class CreateEmisorCommandValidator : AbstractValidator<CreateEmisorCommand>
{
    public CreateEmisorCommandValidator()
    {
        RuleFor(x => x.Ruc)
            .NotEmpty().WithMessage("El RUC es obligatorio.")
            .Length(13).WithMessage("El RUC debe tener exactamente 13 dígitos.")
            .Must(RucValidator.EsValido).WithMessage("El RUC ingresado no es válido.");

        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("La razón social es obligatoria.")
            .MaximumLength(300).WithMessage("La razón social no puede exceder 300 caracteres.");

        RuleFor(x => x.Direccion)
            .NotEmpty().WithMessage("La dirección es obligatoria.");

        RuleFor(x => x.SerieEstablecimiento)
            .NotEmpty().Matches(@"^\d{3}$").WithMessage("Serie de establecimiento debe ser 3 dígitos (ej. 001).");

        RuleFor(x => x.SeriePuntoEmision)
            .NotEmpty().Matches(@"^\d{3}$").WithMessage("Serie de punto de emisión debe ser 3 dígitos (ej. 001).");
    }
}
