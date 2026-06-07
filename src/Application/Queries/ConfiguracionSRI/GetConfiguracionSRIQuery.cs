using Application.DTOs.ConfiguracionSRI;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Queries.ConfiguracionSRI;

public record GetConfiguracionSRIQuery(long EmisorId) : IRequest<ConfiguracionSRIDto?>;

public class GetConfiguracionSRIQueryHandler : IRequestHandler<GetConfiguracionSRIQuery, ConfiguracionSRIDto?>
{
    private readonly IConfiguracionSRIRepository _configRepo;
    private readonly IMapper _mapper;

    public GetConfiguracionSRIQueryHandler(IConfiguracionSRIRepository configRepo, IMapper mapper)
    {
        _configRepo = configRepo;
        _mapper = mapper;
    }

    public async Task<ConfiguracionSRIDto?> Handle(GetConfiguracionSRIQuery request, CancellationToken cancellationToken)
    {
        var config = await _configRepo.GetActivaPorEmisorAsync(request.EmisorId);
        return config is null ? null : _mapper.Map<ConfiguracionSRIDto>(config);
    }
}
