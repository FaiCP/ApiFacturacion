namespace Application.DTOs.Custodios;

public class CustodioDto
{
    public long Id { get; set; }
    public long IdDepartamento { get; set; }
    public string CedulaEmpleado { get; set; } = string.Empty;
    public string CargoEmpleado { get; set; } = string.Empty;
    public string NombreEmpleado { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
}
