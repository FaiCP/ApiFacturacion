using Domain.Interfaces;
using MediatR;

namespace Application.Commands.Hardware;

public record DeleteHardwareCommand(List<long> Ids) : IRequest<bool>;

public class DeleteHardwareCommandHandler : IRequestHandler<DeleteHardwareCommand, bool>
{
    private readonly IHardwareRepository _hardwareRepository;

    public DeleteHardwareCommandHandler(IHardwareRepository hardwareRepository)
    {
        _hardwareRepository = hardwareRepository;
    }

    public async Task<bool> Handle(DeleteHardwareCommand request, CancellationToken cancellationToken)
    {
        foreach (var id in request.Ids)
        {
            var hardware = await _hardwareRepository.GetByIdAsync(id);
            if (hardware != null)
            {
                hardware.Borrado = true;
                hardware.UpdatedAt = DateTime.UtcNow;
                await _hardwareRepository.UpdateAsync(hardware);
            }
        }
        return true;
    }
}
