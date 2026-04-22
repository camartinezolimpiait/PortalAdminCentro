using portalAdministrativoSISEC.Data.Pines;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo
{
    public static class PagoPinConst
    {
        #region Descripcion Enums

        internal static readonly Dictionary<EnumTramite, string> DescripcionTramite = new()
        {
            { EnumTramite.SinDefinir, "Trámite no definido"},
            { EnumTramite.PrimeraVez, "Primera vez o licencia adicional"},
            { EnumTramite.Renovar, "Renovar licencia"},
            { EnumTramite.Recategorizar, "Recategorizar licencia"},
            { EnumTramite.PrimeraVezInstructor, "Nueva licencia de instructor"},
            { EnumTramite.RecategorizarInstructor, "Recategorizar licencia de instructor"}
        };

        internal static readonly Dictionary<EnumTipoDocumento, string> DescripcionDocumento = new()
        {
            { EnumTipoDocumento.CedulaCiudadania, "Cédula de Ciudadanía"},
            { EnumTipoDocumento.CedulaExtranjeria, "Cédula de Extranjería"},
            { EnumTipoDocumento.TarjetaIdentidad, "Tarjeta de Identidad"},
            { EnumTipoDocumento.Nit, "Nit"},
            { EnumTipoDocumento.Pasaporte, "Pasaporte"},
            { EnumTipoDocumento.ContrasenaCedulaCiudadania, "Contraseña Cédula de Ciudadanía"},
            { EnumTipoDocumento.ContrasenaCedulaExtranjeria, "Contraseña Cédula de Extranjería"},
            { EnumTipoDocumento.PermisoPorProteccionTemporal, "Permiso de protección temporal"}

        };

        internal static readonly Dictionary<EnumEstadoPin, string> DescripcionEstadoPin = new()
        {
            { EnumEstadoPin.Referencia, "Referencia generada sin recaudo."},
            { EnumEstadoPin.Activo, "Pin activo para uso en el enrolamiento."},
            { EnumEstadoPin.Anulado, "Referencia anulada."},
            { EnumEstadoPin.Usado, "Referencia Usada."},
            { EnumEstadoPin.CambioBeneficiario, "Solicitud cambio de documento por función cambio beneficiario."},
            { EnumEstadoPin.ReferenciaVencida, "Referencia Vencida."},
        };

        internal static readonly Dictionary<EnumOrigenCotizacion, string> DescripcionCanalVenta = new()
        {
            { EnumOrigenCotizacion.Centro , "Centro.milicencia" },
            { EnumOrigenCotizacion.MiLicencia , "Milicencia.co" },
            { EnumOrigenCotizacion.SUSER , "Super Servicios"},
            { EnumOrigenCotizacion.PAO , "Pao" },
            { EnumOrigenCotizacion.PortalAdministrativo , "Portal Administrativo" },
            { EnumOrigenCotizacion.MigracionCEA , "Migración Cea"}
        };

        #endregion Descripcion Enums

        #region Pines

        internal static List<ColumnList> InicializarColumnasDevoluciones()
        {
            return [
                new() { NameColumn = "Canal de venta"},
                new() { NameColumn = "Pin"},
                new(){ NameColumn = "Código de transacción" },
                new() { NameColumn = "Número Documento"},
                new() { NameColumn = "Nombre Completo"},
                new() { NameColumn = "Fecha Registro"},
                new() { NameColumn = "Fecha Devolución"},
                new() { NameColumn = "Tipo Devolución"},
                new() { NameColumn = "Banco"},
                new() { NameColumn = "Cuenta Banco"},
                new() { NameColumn = "Correo"},
                new() { NameColumn = "Valor a Devolver"},
                new() { NameColumn = "Estado Devolución"},
                new() { NameColumn = "Novedad"},
                new() { NameColumn = "Agente Dispersión"},
                new() { NameColumn = "Pines Asociados" },
                new() { NameColumn = "Cuotas" },
                new() { NameColumn = "Comprobante Devolución"}
            ];
        }

        internal static List<ColumnList> InicializarColumnasActivos()
        {
            return [
                new(){ NameColumn = "Canal de Venta" },
                new(){ NameColumn = "Pin" },
                new(){ NameColumn = "Código de transacción" },
                new(){ NameColumn = "Tipo Documento" },
                new(){ NameColumn = "Número Documento" },
                new(){ NameColumn = "Valor Pin" },
                new(){ NameColumn = "Valor Actor" },
                new(){ NameColumn = "Valor ANSV" },
                new(){ NameColumn = "Valor Aliado" },
                new(){ NameColumn = "Valor Sicov" },
                new(){ NameColumn = "Fecha Operación" },
                new(){ NameColumn = "Agente Dispersión" },
                new(){ NameColumn = "Razón Social" },
                new(){ NameColumn = "Tipo Pin" },
                new(){ NameColumn = "Pines Asociados" },
				//new(){ NameColumn = "Cuotas" }
			];
        }

        internal static List<ColumnList> InicializarColumnasDispersiones()
        {
            return [
                new(){ NameColumn = "IdDispersion" },
                new(){ NameColumn = "Negocio" },
                new(){ NameColumn = "Banco" },
                new(){ NameColumn = "Cuenta" },
                new(){ NameColumn = "Total Dispersión" },
                new(){ NameColumn = "Agente Dispersión" },
                new(){ NameColumn = "Detalle de Pago" },
            ];
        }

        internal static List<ColumnList> InicializarColumnasAsociados()
        {
            return [
                new(){ NameColumn = "Pin" },
                new(){ NameColumn = "Tipo Documento" },
                new(){ NameColumn = "Número Documento" },
                new(){ NameColumn = "Valor Pin" },
                new(){ NameColumn = "Valor Actor" },
                new(){ NameColumn = "Valor ANSV" },
                new(){ NameColumn = "Valor Aliado" },
                new(){ NameColumn = "Valor Sicov" },
                new(){ NameColumn = "Fecha Operación" },
                new(){ NameColumn = "Fecha Dispersión" },
                new(){ NameColumn = "Banco Dispersión" },
                new(){ NameColumn = "Cuenta Dispersión" },
                new(){ NameColumn = "Valor Dispersión" },
                new(){ NameColumn = "Agente Dispersión" },
                new(){ NameColumn = "Razón Social" },
                new(){ NameColumn = "Tipo Pin" }
            ];
        }

        internal static List<ColumnList> InicializarColumnasAsociadosPinDirecto()
        {
            return [
                new(){ NameColumn = "Pin" },
                new(){ NameColumn = "Tipo Documento" },
                new(){ NameColumn = "Número Documento" },
                new(){ NameColumn = "Valor Pin" },
                new(){ NameColumn = "Valor Actor" },
                new(){ NameColumn = "Valor Aliado" },
                new(){ NameColumn = "Fecha pago" },
                new(){ NameColumn = "Agente Dispersión" },
            ];
        }

        internal static List<ColumnList> InicializarColumnasUsados()
        {
            return [
                new ColumnList { NameColumn = "Canal de Venta" },
                new ColumnList { NameColumn = "Pin" },
                new ColumnList{ NameColumn = "Código de transacción" },
                new ColumnList { NameColumn = "Tipo Documento" },
                new ColumnList { NameColumn = "Número Documento" },
                new ColumnList { NameColumn = "Valor Pin" },
                new ColumnList { NameColumn = "Valor Actor" },
                new ColumnList { NameColumn = "Valor ANSV" },
                new ColumnList { NameColumn = "Valor Aliado" },
                new ColumnList { NameColumn = "Valor Sicov" },
                new ColumnList { NameColumn = "Fecha Operación" },
                new ColumnList { NameColumn = "IdDispersión" },
                new ColumnList { NameColumn = "Fecha Dispersión" },
                new ColumnList { NameColumn = "Banco Dispersión" },
                new ColumnList { NameColumn = "Cuenta Dispersión" },
                new ColumnList { NameColumn = "Valor Dispersión" },
                new ColumnList { NameColumn = "Agente Dispersión" },
                new ColumnList { NameColumn = "Razón Social" },
                new ColumnList { NameColumn = "Tipo Pin" },
                new ColumnList { NameColumn = "Pines Asociados" },
				//new ColumnList { NameColumn = "Cuotas Pin" }
			];
        }

        #endregion Pines
    }
}