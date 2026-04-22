using portalAdministrativoSISEC.Enum.Devoluciones;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Entidades.Devolucion
{
    public class PaymentOptionModel : IValidatableObject
    {
        public PaymentType PaymentType { get; set; }

        [Range(50000, double.MaxValue, ErrorMessage = "El valor mínimo permitido es de $50.000.")]
        public decimal? ValorParcial { get; set; }

        public int? Cuotas { get; set; } = 1;
        public int? CuotasRestantes { get; set; }
        public decimal? ValorCuota { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal MontoAPagar =>
            PaymentType switch
            {
                PaymentType.Cuotas => (ValorTotal) / (CuotasRestantes ?? 0) * (Cuotas ?? 0),
                PaymentType.Parcial => ValorParcial ?? 0,
                PaymentType.Total => ValorTotal,
                _ => 0
            };

        // Validación personalizada
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PaymentType == PaymentType.Parcial)
            {
                if (ValorParcial.HasValue && ValorParcial.Value > ValorTotal)
                {
                    yield return new ValidationResult(
                        "El valor parcial no puede ser mayor al valor total.",
                        new[] { nameof(ValorParcial) }
                    );
                }

                if (!ValorParcial.HasValue)
                {
                    yield return new ValidationResult(
                        "El valor parcial no puede ser vacío.",
                        new[] { nameof(ValorParcial) }
                    );
                }
            }

            if (PaymentType == PaymentType.Cuotas && Cuotas > CuotasRestantes)
            {
                yield return new ValidationResult(
                        "Las cuotas seleccionadas no son validas.",
                        new[] { nameof(ValorParcial) }
                    );
            }
        }
    }

}
