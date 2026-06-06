using Application.Common;
using Application.DTOs.NotasDebito;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.NotasDebito;

public record GetNotasDebitoQuery(int Cantidad, int Pagina, EstadoSRI? Estado, long? FacturaId) : IRequest<PaginatedResponse<NotaDebitoDto>>;

public class GetNotasDebitoQueryHandler : IRequestHandler<GetNotasDebitoQuery, PaginatedResponse<NotaDebitoDto>>
{
    private readonly INotaDebitoRepository _ndRepo;
    private readonly IMapper _mapper;
    public GetNotasDebitoQueryHandler(INotaDebitoRepository ndRepo, IMapper mapper) { _ndRepo = ndRepo; _mapper = mapper; }

    public async Task<PaginatedResponse<NotaDebitoDto>> Handle(GetNotasDebitoQuery query, CancellationToken ct)
    {
        var (items, total) = await _ndRepo.GetPagedAsync(query.Pagina, query.Cantidad, query.Estado, query.FacturaId);
        return new PaginatedResponse<NotaDebitoDto>
        {
            Items = _mapper.Map<List<NotaDebitoDto>>(items),
            TotalCount = total, PageNumber = query.Pagina, PageSize = query.Cantidad
        };
    }
}
