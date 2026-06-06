using Application.DTOs.Auth;
using Application.DTOs.Clientes;
using Application.DTOs.ConfiguracionSRI;
using Application.DTOs.Emisor;
using Application.DTOs.Facturas;
using Application.DTOs.NotasCredito;
using Application.DTOs.NotasDebito;
using Application.DTOs.Productos;
using Application.DTOs.Retenciones;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── FACTURACIÓN ELECTRÓNICA ──────────────────────────────────────

        CreateMap<Domain.Entities.Emisor, EmisorDto>()
            .ForMember(dest => dest.Ambiente, opt => opt.MapFrom(src => src.Ambiente.ToString()));

        CreateMap<Domain.Entities.Cliente, ClienteDto>()
            .ForMember(dest => dest.TipoIdentificacion, opt => opt.MapFrom(src => src.TipoIdentificacion.ToString()));

        CreateMap<Domain.Entities.Producto, ProductoDto>()
            .ForMember(dest => dest.TarifaIva, opt => opt.MapFrom(src => src.TarifaIva.ToString()))
            .ForMember(dest => dest.PorcentajeIva, opt => opt.MapFrom(src =>
                src.TarifaIva == TarifaIva.Quince ? 15m :
                src.TarifaIva == TarifaIva.Cero ? 0m : 0m));

        // Factura
        CreateMap<Domain.Entities.Factura, FacturaDto>()
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente))
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));

        CreateMap<Domain.Entities.Factura, FacturaResumenDto>()
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.NumeroCompleto, opt => opt.MapFrom(src => $"{src.Serie}-{src.Secuencial}"))
            .ForMember(dest => dest.RazonSocialCliente, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.RazonSocial : ""))
            .ForMember(dest => dest.IdentificacionCliente, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.NumeroIdentificacion : ""));

        CreateMap<Domain.Entities.DetalleFactura, DetalleFacturaDto>()
            .ForMember(dest => dest.TarifaIva, opt => opt.MapFrom(src => src.TarifaIva.ToString()));

        // Nota de Crédito
        CreateMap<Domain.Entities.NotaCredito, NotaCreditoDto>()
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente))
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));
        CreateMap<Domain.Entities.DetalleNotaCredito, DetalleFacturaDto>()
            .ForMember(dest => dest.TarifaIva, opt => opt.MapFrom(src => src.TarifaIva.ToString()));

        // Nota de Débito
        CreateMap<Domain.Entities.NotaDebito, NotaDebitoDto>()
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente))
            .ForMember(dest => dest.Motivos, opt => opt.MapFrom(src => src.Motivos));
        CreateMap<Domain.Entities.MotivoNotaDebito, MotivoDto>();

        // Retención
        CreateMap<Domain.Entities.Retencion, RetencionDto>()
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.SujetoRetenido, opt => opt.MapFrom(src => src.SujetoRetenido))
            .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));
        CreateMap<Domain.Entities.DetalleRetencion, DetalleRetencionDto>()
            .ForMember(dest => dest.TipoImpuesto, opt => opt.MapFrom(src => src.TipoImpuesto.ToString()));

        // ConfiguracionSRI
        CreateMap<Domain.Entities.ConfiguracionSRI, ConfiguracionSRIDto>()
            .ForMember(dest => dest.Ambiente, opt => opt.MapFrom(src => src.Ambiente.ToString()))
            .ForMember(dest => dest.TieneCertificado, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.CertificadoBase64)));

        // Usuario -> LoginResponseDto (manual in handler but keep mapping for completeness)
        CreateMap<Usuario, LoginResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForMember(dest => dest.Cargo, opt => opt.MapFrom(src => src.Cargo ?? string.Empty));
    }
}
