using Application.DTOs.Emisor;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Emisor;

public record GetEmisorByIdQuery(long Id) : IRequest<EmisorDto>;

public class GetEmisorByIdQueryHandler : IRequestHandler<GetEmisorByIdQuery, EmisorDto>
{
    private readonly IEmisorRepository _emisorRepository;
    private readonly IMapper _mapper;

    public GetEmisorByIdQueryHandler(IEmisorRepository emisorRepository, IMapper mapper)
    {
        _emisorRepository = emisorRepository;
        _mapper = mapper;
    }

    public async Task<EmisorDto> Handle(GetEmisorByIdQuery request, CancellationToken cancellationToken)
    {
        var emisor = await _emisorRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Emisor {request.Id} no encontrado.");
        return _mapper.Map<EmisorDto>(emisor);
    }
}
