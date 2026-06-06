using Application.DTOs.NotasCredito;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.NotasCredito;

public record GetNotaCreditoByIdQuery(long Id) : IRequest<NotaCreditoDto>;

public class GetNotaCreditoByIdQueryHandler : IRequestHandler<GetNotaCreditoByIdQuery, NotaCreditoDto>
{
    private readonly INotaCreditoRepository _ncRepo;
    private readonly IMapper _mapper;
    public GetNotaCreditoByIdQueryHandler(INotaCreditoRepository ncRepo, IMapper mapper) { _ncRepo = ncRepo; _mapper = mapper; }

    public async Task<NotaCreditoDto> Handle(GetNotaCreditoByIdQuery request, CancellationToken ct)
    {
        var nc = await _ncRepo.GetWithDetallesAsync(request.Id)
            ?? throw new NotFoundException($"Nota de Crédito {request.Id} no encontrada.");
        return _mapper.Map<NotaCreditoDto>(nc);
    }
}
