namespace DBCafeteria.Domain.Entities;

public sealed class Vaso
{
    public int Id { get; set; }
    public string? Clave { get; set; }
    public string? Descripcion { get; set; }
    public int? ClaveNumerica { get; set; }
    public byte[]? Imagen { get; set; }
    public bool BebidasFrias { get; set; }
    public bool BebidasCalientes { get; set; }
    public int Existencia { get; set; }
}
