using Domain.Enums;

namespace Application.Services;

public static class TaxCalculatorService
{
    public static decimal ObtenerPorcentaje(TarifaIva tarifa) => tarifa switch
    {
        TarifaIva.Quince => 0.15m,
        TarifaIva.Cero   => 0m,
        _                => 0m   // Exento / NoObjeto no generan IVA
    };

    public static LineaTotales CalcularLinea(decimal cantidad, decimal precioUnitario, decimal descuento, TarifaIva tarifa)
    {
        var subtotal = Math.Round(cantidad * precioUnitario, 2);
        var subtotalSinImpuesto = Math.Round(subtotal - descuento, 2);
        var valorIva = Math.Round(subtotalSinImpuesto * ObtenerPorcentaje(tarifa), 2);
        return new LineaTotales(subtotalSinImpuesto, valorIva, subtotalSinImpuesto);
    }

    public static FacturaTotales CalcularTotales(IEnumerable<LineaTotales> lineas, decimal descuentoTotal = 0)
    {
        var lista = lineas.ToList();
        var totalSinImpuestos = lista.Sum(l => l.SubtotalSinImpuesto);
        var totalIva = lista.Sum(l => l.ValorIva);
        return new FacturaTotales(
            Math.Round(totalSinImpuestos, 2),
            Math.Round(descuentoTotal, 2),
            Math.Round(totalIva, 2),
            Math.Round(totalSinImpuestos + totalIva, 2));
    }
}

public record LineaTotales(decimal SubtotalSinImpuesto, decimal ValorIva, decimal PrecioTotalSinImpuesto);

public record FacturaTotales(decimal TotalSinImpuestos, decimal TotalDescuento, decimal TotalIva, decimal ImporteTotal);
