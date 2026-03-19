using Application.Common;
using Application.DTOs.Hardware;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.Hardware;

public record GetHardwareQuery(int Cantidad, int Pagina, string Busqueda) : IRequest<PaginatedResponse<HardwareDto>>;

public class GetHardwareQueryHandler : IRequestHandler<GetHardwareQuery, PaginatedResponse<HardwareDto>>
{
    private readonly IHardwareRepository _hardwareRepository;
    private readonly IMapper _mapper;

    public GetHardwareQueryHandler(IHardwareRepository hardwareRepository, IMapper mapper)
    {
        _hardwareRepository = hardwareRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<HardwareDto>> Handle(GetHardwareQuery query, CancellationToken cancellationToken)
    {
        var pageNumber = query.Pagina <= 0 ? 1 : query.Pagina;
        var items = await _hardwareRepository.SearchAsync(query.Busqueda, pageNumber, query.Cantidad);
        var total = await _hardwareRepository.CountSearchAsync(query.Busqueda);

        return new PaginatedResponse<HardwareDto>
        {
            Items = _mapper.Map<List<HardwareDto>>(items),
            TotalCount = total,
            PageNumber = query.Pagina,
            PageSize = query.Cantidad
        };
    }
}
