using Application.DTOs.Auth;
using Application.DTOs.Custodios;
using Application.DTOs.Departamentos;
using Application.DTOs.GestionActivos;
using Application.DTOs.Hardware;
using Application.DTOs.HistorialPrestamos;
using Application.DTOs.Kits;
using Application.DTOs.Personal;
using Application.DTOs.Suministros;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Departamento
        CreateMap<Departamento, DepartamentoDto>().ReverseMap();

        // Persona / Personal
        CreateMap<Persona, PersonalDto>().ReverseMap();

        // Custodio
        CreateMap<Custodio, CustodioDto>()
            .ForMember(dest => dest.NombreEmpleado, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.CedulaEmpleado, opt => opt.MapFrom(src => src.Cedula))
            .ForMember(dest => dest.CargoEmpleado, opt => opt.MapFrom(src => src.Cargo))
            .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.IdDepartamento ?? 0))
            .ForMember(dest => dest.Departamento, opt => opt.MapFrom(src => src.Departamento != null ? src.Departamento.Nombre : string.Empty));

        CreateMap<CustodioDto, Custodio>()
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.NombreEmpleado))
            .ForMember(dest => dest.Cedula, opt => opt.MapFrom(src => src.CedulaEmpleado))
            .ForMember(dest => dest.Cargo, opt => opt.MapFrom(src => src.CargoEmpleado));

        // Hardware
        CreateMap<Hardware, HardwareDto>()
            .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Observacion))
            .ForMember(dest => dest.FechaAdquisicion, opt => opt.MapFrom(src =>
                src.FechaAdquisicion.HasValue
                    ? DateOnly.FromDateTime(src.FechaAdquisicion.Value)
                    : (DateOnly?)null))
            .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor.HasValue ? (double?)Convert.ToDouble(src.Valor.Value) : null))
            .ForMember(dest => dest.NombreCustodio, opt => opt.MapFrom(src =>
                src.GestionActivos.FirstOrDefault(g => g.FechaDevolucion == null) != null
                    ? src.GestionActivos.FirstOrDefault(g => g.FechaDevolucion == null)!.Custodio!.Nombre
                    : string.Empty))
            .ForMember(dest => dest.Departamento, opt => opt.Ignore())
            .ForMember(dest => dest.Ram, opt => opt.Ignore())
            .ForMember(dest => dest.Rom, opt => opt.Ignore())
            .ForMember(dest => dest.Procesador, opt => opt.Ignore());

        // CaracteristicaComputadora
        CreateMap<CaracteristicaComputadora, CaracteristicaComputadoraDto>().ReverseMap();

        // Kit
        CreateMap<Kit, KitDto>().ReverseMap();

        // Suministro
        CreateMap<Suministro, SuministroDto>().ReverseMap();

        // GestionActivo
        CreateMap<GestionActivo, GestionActivoDto>()
            .ForMember(dest => dest.NombreEmpleado, opt => opt.MapFrom(src => src.Custodio != null ? src.Custodio.Nombre : string.Empty))
            .ForMember(dest => dest.IdDepartamento, opt => opt.MapFrom(src => src.Custodio != null ? src.Custodio.IdDepartamento : null))
            .ForMember(dest => dest.Marca, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.Marca : string.Empty))
            .ForMember(dest => dest.Modelo, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.Modelo : string.Empty))
            .ForMember(dest => dest.FechaAdquisicion, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.FechaAdquisicion : null))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.Estado : string.Empty))
            .ForMember(dest => dest.Ubicacion, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.Ubicacion : string.Empty))
            .ForMember(dest => dest.CodigoCne, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.CodigoCne : string.Empty))
            .ForMember(dest => dest.NombreDispositivo, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.NombreDispositivo : string.Empty));

        // HistorialPrestamo (maps from GestionActivo)
        CreateMap<GestionActivo, HistorialPrestamoDto>()
            .ForMember(dest => dest.NombreEmpleado, opt => opt.MapFrom(src => src.Custodio != null ? src.Custodio.Nombre : string.Empty))
            .ForMember(dest => dest.Marca, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.Marca : string.Empty))
            .ForMember(dest => dest.Modelo, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.Modelo : string.Empty))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.Estado : string.Empty))
            .ForMember(dest => dest.CodigoCne, opt => opt.MapFrom(src => src.Hardware != null ? src.Hardware.CodigoCne : string.Empty));

        // Usuario -> LoginResponseDto (manual in handler but keep mapping for completeness)
        CreateMap<Usuario, LoginResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForMember(dest => dest.Cargo, opt => opt.MapFrom(src => src.Cargo ?? string.Empty));
    }
}
