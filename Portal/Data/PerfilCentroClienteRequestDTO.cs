// Inicio código generado por GitHub Copilot
using System;

namespace portalAdministrativoSISEC.Data
{
 /// <summary>
 /// DTO para solicitudes relacionadas con perfil, centro, runt, tipo de cliente y usuario.
 /// </summary>
 public class PerfilCentroClienteRequestDTO
 {
 public int IdPerfil { get; set; }
 public int IdCentro { get; set; }
 public string IdRunt { get; set; }
 public int IdTipoCliente { get; set; }
 public string Usuario { get; set; }
 public Guid TransaccionGuid { get; set; }
 }
}
// Fin código generado por GitHub Copilot
