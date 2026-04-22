using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Pages.Facturacion.Models;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util.Const.Facturacion;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util.Extension
{
    public static class FacturacionElectronicaExtension
    {
        #region Fields

        private static DatosArticulos DatosArticulos;

        #endregion Fields

        #region Public Methods

        public static async Task ShowErrorMessageCollection<T>(this FacturacionResponse<T> facturacionResponse, IMiLicenciaService miLicenciaService, string prefijo)
        {
            facturacionResponse.Errores ??= [];

            facturacionResponse.Errores.Add(facturacionResponse.Mensaje);

            if (!facturacionResponse.SolicitudExitosa)
                facturacionResponse.Errores.ForEach(async x => await miLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"{prefijo}: {x}"));

            await Task.FromResult(true);
        }

        public static async Task<ConfigurarArticulosModel> CargarDatosModelo(this DatosArticulos datosArticulos)
        {
            DatosArticulos = datosArticulos;

            ConfigurarArticulosModel articulosModel = new()
            {
                AplicaConfiguracionEspecifica = datosArticulos.AplicaConfiguracionEspecifica,
                TarifaAnsv = await ObtenerCodigoArticulo("Impuesto ANSV"),
                TarifaSicov = await ObtenerCodigoArticulo("Tarifa del SICOV"),
                TarifaAliado = await ObtenerCodigoArticulo("Tarifa del aliado de recaudo"),
                CursoConduccion = await ObtenerCodigoArticulo("Curso de conducción"),
                CursoConduccionA1 = await ObtenerCodigoArticulo("A1"),
                CursoConduccionA2 = await ObtenerCodigoArticulo("A2"),
                CursoConduccionB1 = await ObtenerCodigoArticulo("B1"),
                CursoConduccionB2 = await ObtenerCodigoArticulo("B2"),
                CursoConduccionB3 = await ObtenerCodigoArticulo("B3"),
                CursoConduccionC1 = await ObtenerCodigoArticulo("C1"),
                CursoConduccionC2 = await ObtenerCodigoArticulo("C2"),
                CursoConduccionC3 = await ObtenerCodigoArticulo("C3"),
                CursoConduccionRC1 = await ObtenerCodigoArticulo("RC1"),
                CursoConduccionInstructor = await ObtenerCodigoArticulo("Curso de instructor en conducción"),
                CursoConduccionIA1 = await ObtenerCodigoArticulo("IA1"),
                CursoConduccionIA2 = await ObtenerCodigoArticulo("IA2"),
                CursoConduccionIB1 = await ObtenerCodigoArticulo("IB1"),
                CursoConduccionIB2 = await ObtenerCodigoArticulo("IB2"),
                CursoConduccionIB3 = await ObtenerCodigoArticulo("IB3"),
                CursoConduccionIC1 = await ObtenerCodigoArticulo("IC1"),
                CursoConduccionIC2 = await ObtenerCodigoArticulo("IC2"),
                CursoConduccionIC3 = await ObtenerCodigoArticulo("IC3"),
                ExamenMedico = await ObtenerCodigoArticulo("Examen médico"),
                ExamenMedicoSencillo = await ObtenerCodigoArticulo("Examen médico Sencillo"),
                ExamenMedicoCombo = await ObtenerCodigoArticulo("Examen médico Combo")
            };

            return articulosModel;
        }

        #endregion Public Methods

        #region Private Methods

        private static async Task<string> ObtenerCodigoArticulo(string parametro)
        {
            return await Task.FromResult(
                DatosArticulos.DetalleArticulos.FirstOrDefault(x => x.NombreCategoria == parametro || x.NombreArticulo == parametro)?.CodigoArticulo ?? FacturacionConst.DescripcionArticulosFe[parametro]);
        }

        #endregion Private Methods
    }
}