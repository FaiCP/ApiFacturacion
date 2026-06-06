using Application.DTOs.NotasDebito;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.NotasDebito;

public record GetNotaDebitoByIdQuery(long Id) : IRequest<NotaDebitoDto>;

public class GetNotaDebitoByIdQueryHandler : IRequestHandler<GetNotaDebitoByIdQuery, NotaDebitoDto>
{
    private readonly INotaDebitoRepository _ndRepo;
    private readonly IMapper _mapper;
    public GetNotaDebitoByIdQueryHandler(INotaDebitoRepository ndRepo, IMapper mapper) { _ndRepo = ndRepo; _mapper = mapper; }

    public async Task<NotaDebitoDto> Handle(GetNotaDebitoByIdQuery request, CancellationToken ct)
    {
        var nd = await _ndRepo.GetWithMotivosAsync(request.Id)
            ?? throw new NotFoundException($"Nota de Débito {request.Id} no encontrada.");
        return _mapper.Map<NotaDebitoDto>(nd);
    }
}
