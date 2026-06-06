using Application.Commands.Productos;
using FluentValidation;

namespace Application.Validators.Productos;

public class CreateProductoCommandValidator : AbstractValidator<CreateProductoCommand>
{
    public CreateProductoCommandValidator()
    {
        RuleFor(x => x.CodigoPrincipal)
            .NotEmpty().WithMessage("El código principal es obligatorio.")
            .MaximumLength(50).WithMessage("El código no puede exceder 50 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres.");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0.");
    }
}
