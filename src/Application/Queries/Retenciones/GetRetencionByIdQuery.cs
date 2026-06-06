using Application.DTOs.Retenciones;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Retenciones;

public record GetRetencionByIdQuery(long Id) : IRequest<RetencionDto>;

public class GetRetencionByIdQueryHandler : IRequestHandler<GetRetencionByIdQuery, RetencionDto>
{
    private readonly IRetencionRepository _retRepo;
    private readonly IMapper _mapper;
    public GetRetencionByIdQueryHandler(IRetencionRepository retRepo, IMapper mapper) { _retRepo = retRepo; _mapper = mapper; }

    public async Task<RetencionDto> Handle(GetRetencionByIdQuery request, CancellationToken ct)
    {
        var ret = await _retRepo.GetWithDetallesAsync(request.Id)
            ?? throw new NotFoundException($"Retención {request.Id} no encontrada.");
        return _mapper.Map<RetencionDto>(ret);
    }
}
