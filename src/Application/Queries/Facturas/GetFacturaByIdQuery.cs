using Application.DTOs.Facturas;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Facturas;

public record GetFacturaByIdQuery(long Id) : IRequest<FacturaDto>;

public class GetFacturaByIdQueryHandler : IRequestHandler<GetFacturaByIdQuery, FacturaDto>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IMapper _mapper;

    public GetFacturaByIdQueryHandler(IFacturaRepository facturaRepository, IMapper mapper)
    {
        _facturaRepository = facturaRepository;
        _mapper = mapper;
    }

    public async Task<FacturaDto> Handle(GetFacturaByIdQuery request, CancellationToken cancellationToken)
    {
        var factura = await _facturaRepository.GetWithDetallesAsync(request.Id)
            ?? throw new NotFoundException($"Factura con Id {request.Id} no encontrada.");
        return _mapper.Map<FacturaDto>(factura);
    }
}
