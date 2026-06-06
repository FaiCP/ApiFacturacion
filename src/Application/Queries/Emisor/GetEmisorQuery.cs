using Application.DTOs.Emisor;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Emisor;

public record GetEmisorQuery : IRequest<EmisorDto>;

public class GetEmisorQueryHandler : IRequestHandler<GetEmisorQuery, EmisorDto>
{
    private readonly IEmisorRepository _emisorRepository;
    private readonly IMapper _mapper;

    public GetEmisorQueryHandler(IEmisorRepository emisorRepository, IMapper mapper)
    {
        _emisorRepository = emisorRepository;
        _mapper = mapper;
    }

    public async Task<EmisorDto> Handle(GetEmisorQuery request, CancellationToken cancellationToken)
    {
        var emisor = await _emisorRepository.GetActivoAsync()
            ?? throw new NotFoundException("No existe emisor configurado.");
        return _mapper.Map<EmisorDto>(emisor);
    }
}
