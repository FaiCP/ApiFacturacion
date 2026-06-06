using Application.Commands.Facturas;
using FluentValidation;

namespace Application.Validators.Facturas;

public class CreateFacturaCommandValidator : AbstractValidator<CreateFacturaCommand>
{
    public CreateFacturaCommandValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0).WithMessage("Debe seleccionar un cliente.");

        RuleFor(x => x.Detalles)
            .NotEmpty().WithMessage("La factura debe tener al menos un detalle.");

        RuleForEach(x => x.Detalles).ChildRules(detalle =>
        {
            detalle.RuleFor(d => d.Descripcion)
                .NotEmpty().WithMessage("La descripción del detalle es obligatoria.");

            detalle.RuleFor(d => d.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");

            detalle.RuleFor(d => d.PrecioUnitario)
                .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0.");

            detalle.RuleFor(d => d.Descuento)
                .GreaterThanOrEqualTo(0).WithMessage("El descuento no puede ser negativo.")
                .Must((d, desc) => desc < d.Cantidad * d.PrecioUnitario)
                .WithMessage("El descuento no puede ser mayor o igual al subtotal de la línea.");
        });
    }
}
