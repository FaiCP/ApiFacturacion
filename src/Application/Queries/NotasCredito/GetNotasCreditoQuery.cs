using Application.Common;
using Application.DTOs.NotasCredito;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.NotasCredito;

public record GetNotasCreditoQuery(int Cantidad, int Pagina, EstadoSRI? Estado, long? FacturaId) : IRequest<PaginatedResponse<NotaCreditoDto>>;

public class GetNotasCreditoQueryHandler : IRequestHandler<GetNotasCreditoQuery, PaginatedResponse<NotaCreditoDto>>
{
    private readonly INotaCreditoRepository _ncRepo;
    private readonly IMapper _mapper;
    public GetNotasCreditoQueryHandler(INotaCreditoRepository ncRepo, IMapper mapper) { _ncRepo = ncRepo; _mapper = mapper; }

    public async Task<PaginatedResponse<NotaCreditoDto>> Handle(GetNotasCreditoQuery query, CancellationToken ct)
    {
        var (items, total) = await _ncRepo.GetPagedAsync(query.Pagina, query.Cantidad, query.Estado, query.FacturaId);
        return new PaginatedResponse<NotaCreditoDto>
        {
            Items = _mapper.Map<List<NotaCreditoDto>>(items),
            TotalCount = total, PageNumber = query.Pagina, PageSize = query.Cantidad
        };
    }
}
